# Message-driven architecture
## Урок 1. Принципы асинхронных и синхронных взаимодействий.
1. Воспроизвести приложение по бронированию столика в ресторане из методического указания.
2. Создать возможность снять бронь стола в ресторане синхронно и асинхронно, используя для этого номер забронированного столика.
3. Выделить логику для отправки уведомления в отдельный класс, он будет отвечать за все вопросы связанные с коммуникациями с клиентами, добавить задержку (они будет имитировать создание сообщения) и сделать вызов уведомления асинхронным.
4. Добавить автоматическое “снятие брони”. Например, раз в 20 секунд при наличии забронированных мест - бронь должна сниматься. Асинхронно, независимо от ввода-вывода. Подсказка: можно использовать таймер.
5. (*) Добавить синхронизацию бронирований для множественных асинхронных вызовов и синхронного, это значит, что бронируя столики не дожидаясь предыдущих ответов мы должны получать последовательный результат.

## Урок 2. Advanced Message Queueing Protocol (AMQP).
1. Приложение из первого урока переделать на работу с RabbitMQ. Выделенную в отдельный класс логику отправки уведомления перенести в новое консольное приложение, реализовать взаимодействие через очередь.
2. Переделать работу с очередью на шаблон publisher/subscriber (издатель/подписчик). Сообщения не должны отправляться напрямую в очередь, каждый подписчик должен получить сообщение от издателя. То есть если мы запустим пять сервисов уведомлений, каждый из них должен получить и обработать сообщение https://www.rabbitmq.com/tutorials/tutorial-three-dotnet.html
3. (*) Предложить свою реализацию библиотеки Messaging. Например, если мы захотим использовать не RabbitMQ, а нечто другое это должно быть легко переключаемым. Автоматическое переподключение к брокеру при возникновении проблем на нем. Конфигурация флагов (durable, autoack и т.д). Необходимо мыслить критически и не ограничивать себя в творчестве.

## Урок 3. Работа с MassTransit. 
1. Используя весь опыт предыдущих уроков и код из методички воспроизвести приложение описанное в тексте материала.
2. Добавить в сервис кухни неожиданные поломки или попадание блюд в стоп-лист, которые вызывают реакцию в других сервисах: отправляется уведомление о снятии брони с извинениями и происходит снятие бронирования. Никаких прямых команд между сервисами быть не должно, только данные в публикациях на которые будут реагировать остальные сервисы.
3. (*) Добавить возможность отправлять сообщение через обменник с типом direct из сервиса бронирований на кухню с синхронным ожиданием ответа. Для этого придется использовать заголовки и конфигурацию. Сообщение должно содержать следующий смысл “Когда обед?”. Кухня может отвечать от 0,5 до 3 секунд любым сообщением. Если время ответа превышает 1,5 секунд в более чем 10 случаев из последних 30 - необходимо чтобы срабатывал шаблон “Предохранитель”.