import pika
import time
import sys


def start_mq():
    parameters = pika.ConnectionParameters(host='rabbit')
    connection = pika.BlockingConnection(parameters)
    channel = connection.channel()

    channel.queue_declare(queue='PdfGeneratedMessage', durable=False)

    def callback(ch, method, properties, body):
        sys.stdout.write(" [x] Received %r" % body)
        sys.stdout.flush()
        time.sleep(3)
        
        ch.basic_ack(delivery_tag=method.delivery_tag)

    channel.basic_qos(prefetch_count=1)
    channel.basic_consume(callback,
                          queue='PdfGeneratedMessage')

    channel.start_consuming()
    x = raw_input()

if __name__ == '__main__':
    start_mq()
