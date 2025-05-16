-- 1. Create Roles Table
CREATE DATABASE LMS;
GO

USE LMS;

CREATE TABLE roles (
    role_id INT PRIMARY KEY,
    role_name VARCHAR(50) UNIQUE NOT NULL,
    description VARCHAR(255)
);

-- 2. Create Users Table
CREATE TABLE users (
    user_id INT PRIMARY KEY,
    user_name VARCHAR(100) NOT NULL,
    password VARCHAR(100) NOT NULL,
    email VARCHAR(100) UNIQUE,
    role_id INT,
    registration_timestamp DATETIME DEFAULT SYSDATETIME(),
    last_login_timestamp DATETIME,
    CONSTRAINT fk_users_roles FOREIGN KEY (role_id)
        REFERENCES roles(role_id)
        ON DELETE SET NULL ON UPDATE CASCADE
);

-- 3. Create User Contacts Table
CREATE TABLE user_contacts (
    contact_id INT PRIMARY KEY,
    user_id INT,
    contact VARCHAR(20),
    CONSTRAINT fk_contacts_users FOREIGN KEY (user_id)
        REFERENCES users(user_id)
        ON DELETE CASCADE ON UPDATE CASCADE
);

select * from login_history;

-- 4. Create Login History Table
CREATE TABLE login_history (
    login_id INT PRIMARY KEY,
    user_id INT,
    login_time DATETIME DEFAULT SYSDATETIME(),
    logout_time DATETIME,
    ip_address VARCHAR(50),
    CONSTRAINT fk_login_user FOREIGN KEY (user_id)
        REFERENCES users(user_id)
        ON DELETE CASCADE ON UPDATE CASCADE
);

-- 5. Create Genre Table
CREATE TABLE genre (
    genre_name VARCHAR(50) PRIMARY KEY,
    genre_description VARCHAR(255)
);

-- 6. Create Authors Table
CREATE TABLE authors (
    author_id INT PRIMARY KEY,
    name VARCHAR(100),
    biography VARCHAR(1000),
    date_of_birth DATE
);

select * from login_history;
-- 7. Create Publishers Table
CREATE TABLE publishers (
    publisher_id INT PRIMARY KEY,
    name VARCHAR(100),
    address VARCHAR(255),
    contact_info VARCHAR(100)
);

-- 8. Create Books Table
CREATE TABLE books (
    ISBN VARCHAR(20) PRIMARY KEY,
    title VARCHAR(200) NOT NULL,
    author_id INT,
    publisher_id INT,
    genre_name VARCHAR(50),
    totalCopies INT DEFAULT 1 CHECK (totalCopies >= 0),
    borrowedCopies INT DEFAULT 0 CHECK (borrowedCopies >= 0),
    avlCopies AS (totalCopies - borrowedCopies) PERSISTED,
    publication_year INT,
    CONSTRAINT fk_books_author FOREIGN KEY (author_id)
        REFERENCES authors(author_id)
        ON DELETE SET NULL ON UPDATE CASCADE,
    CONSTRAINT fk_books_publisher FOREIGN KEY (publisher_id)
        REFERENCES publishers(publisher_id)
        ON DELETE SET NULL ON UPDATE CASCADE,
    CONSTRAINT fk_books_genre FOREIGN KEY (genre_name)
        REFERENCES genre(genre_name)
        ON DELETE SET NULL ON UPDATE CASCADE
);

select * from authors;
select * from publishers;
select * from genre;

-- 9. Create Borrowing Record Table
CREATE TABLE borrowing_record (
    borrowing_id INT PRIMARY KEY,
    user_id INT,
    ISBN VARCHAR(20),
    borrow_timestamp DATETIME DEFAULT SYSDATETIME(),
    due_timestamp DATETIME,
    return_timestamp DATETIME,
    status VARCHAR(20) CHECK (status IN ('borrowed', 'returned', 'late')),
    CONSTRAINT fk_borrow_user FOREIGN KEY (user_id)
        REFERENCES users(user_id)
        ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT fk_borrow_book FOREIGN KEY (ISBN)
        REFERENCES books(ISBN)
        ON DELETE CASCADE ON UPDATE CASCADE
);

-- 10. Create Fine Table
CREATE TABLE fine (
    fine_id INT PRIMARY KEY,
    borrowing_id INT,
    fine_amount DECIMAL(10,2) CHECK (fine_amount >= 0),
    fine_due_timestamp DATETIME,
    fine_status VARCHAR(20) CHECK (fine_status IN ('unpaid', 'paid')),
    CONSTRAINT fk_fine_borrow FOREIGN KEY (borrowing_id)
        REFERENCES borrowing_record(borrowing_id)
        ON DELETE CASCADE ON UPDATE CASCADE
);

-- 11. Create Reservation Table
CREATE TABLE reservation (
    reservation_id INT PRIMARY KEY,
    user_id INT,
    ISBN VARCHAR(20),
    reservation_timestamp DATETIME DEFAULT SYSDATETIME(),
    expiration_timestamp DATETIME,
    reservation_status VARCHAR(20) CHECK (reservation_status IN ('active', 'expired', 'cancelled')),
    CONSTRAINT fk_reservation_user FOREIGN KEY (user_id)
        REFERENCES users(user_id)
        ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT fk_reservation_book FOREIGN KEY (ISBN)
        REFERENCES books(ISBN)
        ON DELETE CASCADE ON UPDATE CASCADE
);

select * from borrowing_record;

-- 12. Create Book Reviews Table
CREATE TABLE book_reviews (
    review_id INT PRIMARY KEY,
    ISBN VARCHAR(20),
    user_id INT,
    rating INT CHECK (rating BETWEEN 1 AND 5),
    comment VARCHAR(1000),
    review_timestamp DATETIME DEFAULT SYSDATETIME(),
    CONSTRAINT fk_review_book FOREIGN KEY (ISBN)
        REFERENCES books(ISBN)
        ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT fk_review_user FOREIGN KEY (user_id)
        REFERENCES users(user_id)
        ON DELETE CASCADE ON UPDATE CASCADE
);





-- ROLES
INSERT INTO roles (role_id, role_name, description) VALUES
(1, 'admin', 'System administrator'),
(2, 'member', 'Library member');

-- USERS
INSERT INTO users (user_id, user_name, password, email, role_id, last_login_timestamp) VALUES
(101, 'Alice Johnson', 'alice123', 'alice@example.com', 2, GETDATE()),
(102, 'Bob Smith', 'bobsecure', 'bob@example.com', 1, GETDATE());

-- USER CONTACTS
INSERT INTO user_contacts (contact_id, user_id, contact) VALUES
(1, 101, '123-456-7890'),
(2, 102, '987-654-3210');

-- LOGIN HISTORY
INSERT INTO login_history (login_id, user_id, login_time, logout_time, ip_address) VALUES
(1, 101, GETDATE(), NULL, '192.168.1.10'),
(2, 102, GETDATE(), NULL, '192.168.1.11');

-- GENRE
INSERT INTO genre (genre_name, genre_description) VALUES
('Science Fiction', 'Futuristic or scientific settings'),
('Mystery', 'Detective and crime-solving stories');

-- AUTHORS
INSERT INTO authors (author_id, name, biography, date_of_birth) VALUES
(201, 'Isaac Asimov', 'Prolific science fiction author', '1920-01-02'),
(202, 'Agatha Christie', 'Queen of mystery novels', '1890-09-15');

-- PUBLISHERS
INSERT INTO publishers (publisher_id, name, address, contact_info) VALUES
(301, 'Penguin Books', '123 Penguin Lane', 'contact@penguin.com'),
(302, 'HarperCollins', '456 Harper Street', 'info@harpercollins.com');

-- BOOKS
INSERT INTO books (ISBN, title, author_id, publisher_id, genre_name, totalCopies, borrowedCopies, publication_year) VALUES
('9780451524935', 'Foundation', 201, 301, 'Science Fiction', 5, 2, 1951),
('9780007136834', 'Murder on the Orient Express', 202, 302, 'Mystery', 3, 1, 1934);

-- BORROWING RECORD
INSERT INTO borrowing_record (borrowing_id, user_id, ISBN, due_timestamp, return_timestamp, status) VALUES
(401, 101, '9780451524935', DATEADD(DAY, 14, GETDATE()), NULL, 'borrowed'),
(402, 102, '9780007136834', DATEADD(DAY, 14, GETDATE()), NULL, 'borrowed');

-- FINE
INSERT INTO fine (fine_id, borrowing_id, fine_amount, fine_due_timestamp, fine_status) VALUES
(501, 401, 0.00, DATEADD(DAY, 14, GETDATE()), 'unpaid'),
(502, 402, 1.50, DATEADD(DAY, 14, GETDATE()), 'unpaid');

-- RESERVATION
INSERT INTO reservation (reservation_id, user_id, ISBN, expiration_timestamp, reservation_status) VALUES
(601, 101, '9780451524935', DATEADD(DAY, 7, GETDATE()), 'active'),
(602, 102, '9780007136834', DATEADD(DAY, 7, GETDATE()), 'active');

-- BOOK REVIEWS
INSERT INTO book_reviews (review_id, ISBN, user_id, rating, comment) VALUES
(701, '9780451524935', 101, 5, 'Amazing futuristic storytelling!'),
(702, '9780007136834', 102, 4, 'Classic mystery with great twists!');

