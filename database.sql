DROP DATABASE IF EXISTS registration;
CREATE DATABASE registration;

DROP TABLE IF EXISTS users;
CREATE TABLE users (
    id INT PRIMARY KEY IDENTITY(1,1),
    first_name VARCHAR(50) NOT NULL,
    last_name VARCHAR(50) NOT NULL,
    age INT,
    gender CHAR(1) CHECK (gender='m' OR gender='f'),
    phone_no VARCHAR(10),
    email VARCHAR(100)
);
INSERT INTO users(first_name, last_name, age, gender, phone_no, email) VALUES ('manil', 'shah', 21, 'm', '8200962959', 'manilrshah@ggmail.com');

DROP PROCEDURE IF EXISTS getAllUsers;
CREATE PROCEDURE getAllUsers
AS
BEGIN
	SELECT * FROM users;
END;

DROP PROCEDURE IF EXISTS updateUser;
CREATE PROCEDURE updateUser @emp_id INT, @new_name VARCHAR(50), @new_surname VARCHAR(50), @new_age INT, @new_gender  CHAR(1), @new_no VARCHAR(10), @new_email VARCHAR(100), @new_pass VARCHAR(100)
AS
BEGIN
	UPDATE users SET first_name=@new_name, last_name=@new_surname, age=@new_age, gender=@new_gender, phone_no=@new_no, email=@new_email
	WHERE id = @emp_id;	 

	UPDATE permissions SET password = @new_pass
	WHERE emp_id = @emp_id;
END;

DROP PROCEDURE IF EXISTS addUser;
CREATE PROCEDURE addUser @new_name VARCHAR(50), @new_surname VARCHAR(50), @new_age INT, @new_gender  CHAR(1), @new_no VARCHAR(10), @new_email VARCHAR(100), @emp_role VARCHAR(20), @pass VARCHAR(100)
AS
BEGIN
	INSERT INTO users (first_name, last_name, age, gender, phone_no, email) VALUES (@new_name, @new_surname, @new_age, @new_gender, @new_no, @new_email);
	DECLARE @user_id INT;
	SELECT @user_id = SCOPE_IDENTITY();
	INSERT INTO permissions (emp_id, role, password) VALUES (@user_id, @emp_role, @pass);
END;

DROP PROCEDURE IF EXISTS getEmpById;
CREATE PROCEDURE getEmpById @emp_id INT
AS
BEGIN
	SELECT * FROM users WHERE id = @emp_id;
END;

DROP TABLE IF EXISTS permissions;
CREATE TABLE permissions (
	id INT PRIMARY KEY IDENTITY(1, 1),
	emp_id INT,
	role varchar(20) CHECK (role='admin' OR role='user'),
	FOREIGN KEY (emp_id) REFERENCES users(id)
);
INSERT INTO permissions (emp_id, role) VALUES (1, 'admin'), (2, 'admin'), (3, 'user'), (4, 'user'), (5, 'user');

ALTER TABLE permissions 
ADD password VARCHAR(100);

UPDATE permissions SET password = 'manil' where id = 1;
UPDATE permissions SET password = 'mihir' where id = 2;
UPDATE permissions SET password = 'yash' where id = 3;
UPDATE permissions SET password = 'mohit' where id = 4;
UPDATE permissions SET password = 'yashvi' where id = 5;

CREATE PROCEDURE changePass @id INT, @new_pass VARCHAR(100)
AS
BEGIN
	UPDATE permissions SET password = @new_pass WHERE emp_id = @id;	
END;