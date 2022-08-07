# MinecraftDB
A simple minecraft database creator

SQL injection IS possible (but there shouldn't be a problem, unless the world file is third-party)
Use at your own risk

Currently only requires one table called "Stat" with the values: 
category (pk, nchar(100)), world (pk, nchar(50)), mod (pk, nchar(50)), id (pk, nchar(50)), uuid (pk, nchar(36)), value (int)
pk = Primary Key
*NChar values can be highered or lowered, but world and uuid should always have a minimum of 36 characters due to a uuid having 36 characters

