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
![image](https://github.com/user-attachments/assets/7cc4f52a-843f-4cf3-b7a3-6fcc00ca3409)
- Убедитесь, что код для подключения к базе данных PostgreSQL в файле "appsettings.json" выглядит так: "Server=localhost;Port=5432;Database=ZINO_Account_DB;User Id=postgres;Password=111;" по каждому проекту. Это необходимо для корректного подключения к БД.
- Выполните миграцию (команда Add-Migration "m1") и обновите данные в БД (команда Update-Database) с помощью консоли диспетчера пакетов по каждому проекту.
![image](https://github.com/user-attachments/assets/b995cb3a-56a0-4c7e-a448-b743e347fabe)

Если вы выполнили все шаги инструкции, тогда можете запускать программу.
![image](https://github.com/user-attachments/assets/62e933f6-c8c9-4f48-b196-207c8efd3997)
