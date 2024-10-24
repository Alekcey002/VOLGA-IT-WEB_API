# Основное задание: 
1. Account URL: http://localhost:8001/swagger/index.html
2. Hospital URL: http://localhost:8002/swagger/index.html
3. Timetable URL: http://localhost:8003/swagger/index.html
4. Document URL: http://localhost:8004/swagger/index.html
# Дополнительное задание: 
...
# Дополнительная информация:
Здравствуйте! Я разработал программу в рамках вашего задания. Код написан на языке программирования C# в среде Visual Studio. На данный момент выполнена только основная часть. Запустить программу можно просто через кнопку "Пуск" с несколькими начальными проектами, а не через командную строку, так как у меня возникли проблемы с работой Docker. Пожалуйста, следуйте инструкции перед запуском:

- Проверьте, что все проекты, необходимые для запуска, отмечены в свойствах решения "ZINO_IT_VOLGA_2024". 
![image](https://github.com/user-attachments/assets/05c0e7d8-1405-4d6d-b2b0-df07de230121)
- Убедитесь, что код для подключения к базе данных PostgreSQL в файле "appsettings.json" выглядит так: "Server=localhost;Port=5432;Database=ZINO_Account_DB;User Id=postgres;Password=111;" по каждому проекту. Это необходимо для корректного подключения к БД.
- Выполните миграцию (команда Add-Migration "m1") и обновите данные в БД (команда Update-Database) с помощью консоли диспетчера пакетов по каждому проекту.
![image](https://github.com/user-attachments/assets/5110ee11-5f7d-49a3-b822-2a9058e941b3)

Если вы выполнили все шаги инструкции, тогда можете запускать программу.
![image](https://github.com/user-attachments/assets/a24370af-0f9f-4321-9eea-69d19da88b45)
