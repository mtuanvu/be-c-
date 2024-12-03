Project ATM Bank
1. Các thực thể(Entities): User, Account, Transaction

2. Sử dụng asp.net core, Entity Framework core, ORM, SWagger 

3. Cài đặt EF Core và MySQL 
    dotnet add package Microsoft.EntityFrameworkCore
    dotnet add package Microsoft.EntityFrameworkCore.Tools
    dotnet add package Pomelo.EntityFrameworkCore.MySql

4. Cài đặt SWagger để tích hợp API 
    dotnet add package Swashbuckle.AspNetCore

5. Migrations đồng bộ hoá Entities với MySql 
   dotnet ef migrations add InitialCreate  (nếu muốn tạo lại: dotnet restore)

   dotnet ef migrations add AddOtpRecordTable


6. Áp dụng Migration để tạo Database 
    dotnet ef database update 
7. jwt 
 dotnet add package Microsoft.AspNetCore.Authentication.JwtBearerdot


 http://localhost:5005/api/v1/users lấy userId

 http://localhost:5005/api/v1/users/login 

 http://localhost:5005/api/v1/users/register

 http://localhost:5005/api/v1/users/all

 http://localhost:5005/api/v1/users/updata

  http://localhost:5005/api/v1/users/delete
 