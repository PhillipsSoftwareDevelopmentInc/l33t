import pika
import time


def start_mq():
    parameters = pika.ConnectionParameters(host='rabbit')
    connection = pika.BlockingConnection(parameters)
    channel = connection.channel()

    channel.queue_declare(queue='PdfGeneratedMessage', durable=True)

    def callback(ch, method, properties, body):
        print(" [x] Received %r" % body)
        time.sleep(3)
        print(" [x] Done")
        ch.basic_ack(delivery_tag=method.delivery_tag)

    channel.basic_qos(prefetch_count=1)
    channel.basic_consume(callback,
                          queue='PdfGeneratedMessage')

    channel.start_consuming()

if __name__ == '__main__':
    start_mq()
