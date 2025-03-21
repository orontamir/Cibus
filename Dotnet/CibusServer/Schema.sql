-- create data base if not exist
SET @database_name = "CibusDb";
SET @CreateDb =  CONCAT('CREATE DATABASE IF NOT EXISTS `', @database_name, '`');

PREPARE stmp from @createDb;
EXECUTE stmp;
DEALLOCATE PREPARE stmp;


USE CibusDb;

SET @CreateTableUsers = 'CREATE TABLE IF NOT EXISTS Users (
	id int auto_increment primary key,
	name varchar(255) not null,
	password longtext not null,
	constraint users_pk unique (name)
)

';

SET @CreateTableMessages = 'CREATE TABLE IF NOT EXISTS Messages (
	id int auto_increment primary key,
	user_id int not null,
	message VARCHAR(127) null,
	vote int null,
	constraint message_user_id_fk foreign key (user_id) references Users (id)
)

';
PREPARE stmt FROM @createTableUsers;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

PREPARE stmp from @CreateTableMessages;
EXECUTE stmp;
DEALLOCATE PREPARE stmp;

INSERT INTO users (name,password)
SELECT 'Admin', 'DwdkfQ0jAZXX+6HyTR5fOzS0GdXz+PA5h3nkqNTQe+Y='
WHERE NOT EXISTS (SELECT 1 FROM Users WHERE name = 'Admin');