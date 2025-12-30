# API Documentation

Base URL: `http://localhost:5000`

## Authentication Endpoints

### Vault Credentials
- **URL:** `/vault`  
- **Method:** `POST`  
- **Authentication:** None  
- **Description:** Retrieve vault credentials.
```json
{
  "Username": "string",
  "Password": "string"
}
```
200 OK - Secrets applied from vault

500 Error - Failed to fetch secrets from vault

### Login 
- **URL:** `/auth`  
- **Method:** `POST`  
- **Authentication:** None  
- **Description:** Login and retrieve token.
```json
{
  "Username": "string",
  "Password": "string"
}
```

Response:

200 OK - Returns JWT token

401 Unauthorized - Invalid credentials

## Order Endpoints

### Place Order
- **URL:** `/api/order/place-order/{orderId}`  
- **Method:** `POST`  
- **Description:** Start workflow to place order.

Response:

202 Accepted 

### Get Order Status
- **URL:** `/api/order/get-order-status/{orderId}`  
- **Method:** `GET`  
- **Description:** Retrieves order status to confirm order has been placed.

Response:

200 OK - Returns order status


### Get All Orders
- **URL:** `/api/order/getallorders`
- **Method:** `GET`
- **Description:** Retrieves all orders.

Response:

200 OK - Returns all orders

### Get Order By OrderId
- **URL:** `/api/order/getorderbyid/{orderId}`  
- **Method:** `GET`  
- **Description:** Retrieves order by orderId.

Response:

200 OK - Returns order

404 NotFound

### Get Active Order By User 
- **URL:** `/api/order/getactiveorderbyuserid?userId={userId}`  
- **Method:** `GET`  
- **Description:** Retrieves active order by user id.

Response:

200 OK - Returns active order by user id.

### Add Product To Order
- **URL:** `/api/order/addproducttoorder/{orderId}`  
- **Method:** `POST`  
- **Description:** Adds product to order.
```json
{
  "ProductId": 1,
  "Quantity": 2
}
```

Response:

202 Accepted 

### Remove Product From Order
- **URL:** `/api/order/removeproductfromorder/{orderId}/{productId}/{quantity}`  
- **Method:** `DELETE`  
- **Description:** Remove product from order.

Response:

200 OK - Product removed

400 Bad Request - Product/order not found

409 Conflict - Operation cannot be performed

500 Internal Server Error - Other errors

## Stock Endpoints

### Get All Stock
- **URL:** `/api/stock/getallstock`  
- **Method:** `GET`  
- **Description:** Retrieve all stock.

Response:

200 OK - Returns all stock

---

# How to Setup HashiCorp Vault Locally

To set up HashiCorp Vault locally, follow these steps:
1. Run the following command to start a Vault server in development mode:
   ```bash
   docker compose up --build vault
   ```
2. Once the Vault server is running, you can access the Vault UI by navigating to `Localhost:8200` in your web browser.
3. You will be prompted to setup a set of root keys.
4. Type in `3` in `Key Shares` and `2` in `Key Threshold`, then click on `Initialize`.
5. Click `Download keys` to download the unseal keys and root token, or copy them to a secure location
6. Copy paste a Key from the Keys section into the `Unseal Key Portion` input field and click on `Unseal` button.
7. Repeat step 6 with another key to fully unseal the Vault.

**NB: Remember to unseal Vault each time it is restarted.**

8. Once the Vault is unsealed, you can log in using the root token.
9. Click `Secret Engines` in the left sidebar, then click on `Enable new engine`.
10. Select `KV` from the list of secret engines.
11. In the `Path` field, enter `bulls` and click on `Enable engine`.
12. Click on `Create secret`.
13. In the `Path for this secret` field, enter `secret`.
14. Add the following key-value pairs to the secret:
    - `BullsToken`: `a_length_of_at_least_32_characters_is_recommended`
15. Click on `Add` and then `Save` to store the secret.
16. Click on `Policies` in the left sidebar, then click on `Create ACL policy`.
17. In the `Name` field, enter `bulls-user-policy`.
18. In the `Policy` field, enter the following policy:
    ```
    path "bulls/*" {
        capabilities = ["read"]
    }
    ```
19. Click on `Create policy` to save the policy.
20. Click on `Back to main navigation` in the left sidebar, then click on `Access`.
21. Click on `Enable new method` and select `Userpass` from the list of authentication methods.
22. Click on `Enable method` to enable the Userpass authentication method.
23. Click on `Authentication Methods` in the left sidebar, then click on the `userpass/` method.
24. Click on `Create user`.
25. In the `Username` field, enter a username of your choice, but remember it or write it down.
26. In the `Password` field, enter a password of your choice, but remember it or write it down.
27. Click `Save` to create the user.
28. After logging out with the root user Log in with the newly created user to finish the user creation.
29. Log in as root again to continue the setup.
30. Navigate to `Access` > `Entities` in the left sidebar.
32. Click on the entity that was automatically created when you created the user.
33. Click `Edit entity` and search for the `bulls-user-policy` in the `Policies` section and select it.
34. Click on `Save` to attach the policy to the entity.

Now log in with the user credentials you created in Vault, through the vault endpoint:
`localhost:5000/vault`
Then receive your auth token using the auth endpoint with username `John` and password `john123`:
`localhost:5000/auth`

Copying the token from the response, you can now access the secured endpoints by including the token in the `Authorization` header as a Bearer token.

---

# Test Add Product to Order

To test the "Add Product To Order" endpoint, first retrieve stock information by accessing the following endpoint:

`http://localhost:5000/api/stock/getallstock`

Identify a product from the retrieved stock list and note its `ProductId`.

Next, use the noted `ProductId` and  a quantity to add the product to the following endpoint:

`http://localhost:5000/api/order/addproducttoorder/1`

An example of the body could be:

```json
{
  "ProductId": 1,
  "Quantity": 2
}
```

By accessing the following endpoint, you can retrieve an order by its ID:

`http://localhost:5000/api/order/getorderbyid/{orderId}`

To confirm a product has successfully been added to an order.

To validate that a stock has succesfully been retrieved, you can access the following endpoint:

`http://localhost:5000/api/stock/getallstock`

Find the correct product by its ID and confirm that the stock quantity has decreased and the reservedQuantity has increased.

# Test Remove Product from Order

By following the steps above to add a product to an order, you can then test the "Remove Product From Order" endpoint.

`http://localhost:5000/api/order/removeproductfromorder/{orderId}/{productId}/{quantity}`

Use the same `ProductId` and quantity that was used to add the product to the order.

By accessing the following endpoint, you can retrieve an order by its ID:

`http://localhost:5000/api/order/getorderbyid/{orderId}`

To confirm a product has successfully been removed from an order.

To validate that a stock has succesfully been updated after removing a product from an order, you can access the following endpoint:

`http://localhost:5000/api/stock/getallstock`

To validate that a stock has succesfully been updated.

# Test Place an Order


First access the following endpoint to retrieve the order by its ID to confirm the products in the order:

`http://localhost:5000/api/order/getorderbyid/{orderId}`

Next confirm the stock levels for the products in the order by accessing the following endpoint:

`http://localhost:5000/api/stock/getallstock`

To place an order, use the following endpoint:

`http://localhost:5000/api/order/place-order/{orderId}`

After placing the order, you can check the status of the order using the following endpoint:

`http://localhost:5000/api/order/get-order-status/{orderId}`

If the order placing has failed, the endpoint will return a status indicating the failure. To try again run the "Place an order" step again.

If the order is successful, the status will indicate that the order has been placed.

To validate that the stock has been decremented after placing the order, you can access the following endpoint:

`http://localhost:5000/api/stock/getallstock`

Confirm that the stock quantity has decreased and soldQuantity has increased for the products in the order.

# How to setup database

Update StockDb connectionString in the vault to:

`Server=stock-db,1433;Database=StockDb;User Id=StockCreatorLogin;Password={YourPasswordHere};TrustServerCertificate=True`

Add StockServiceDb in the vault:

`Server=stock-db,1433;Database=StockDb;User Id=StockServiceLogin;Password={YourPasswordHere};TrustServerCertificate=True`

Update {YourPasswordHere} to your own password.

Access the database through SQL Server Management Studio:

Servername: localhost,1433  
Login: sa  
Password {YourPasswordHere}

Replace {YourPasswordHere} to your own password defined for the sa user.

Execute the query below. Remember to update {YourPasswordHere} to the passwords used for the connection strings for StockCreatorLogin and StockServiceLogin.

```
USE master;
GO

Create Database StockDb;

CREATE LOGIN StockCreatorLogin
WITH PASSWORD = '{YourPasswordHere}',
     CHECK_POLICY = ON;
GO

CREATE LOGIN StockServiceLogin
WITH PASSWORD = '{YourPasswordHere}',
     CHECK_POLICY = ON;
GO

Use StockDb;
GO

CREATE ROLE StockCreation;
GO

GRANT CREATE TABLE TO StockCreation;
GRANT CREATE VIEW TO StockCreation;
GRANT CREATE PROCEDURE TO StockCreation;
GRANT CREATE FUNCTION TO StockCreation;

GRANT ALTER ON SCHEMA::dbo TO StockCreation;
GRANT REFERENCES TO StockCreation;
GRANT INSERT, UPDATE, DELETE, SELECT ON SCHEMA::dbo TO StockCreation;
GO 

CREATE ROLE StockServiceRW;
GO

CREATE USER StockCreatorUser
FOR LOGIN StockCreatorLogin;
GO

CREATE USER StockServiceUser
FOR LOGIN StockServiceLogin;
GO

ALTER ROLE StockCreation ADD MEMBER StockCreatorUser;
ALTER ROLE StockServiceRW ADD MEMBER StockServiceUser;
GO
```

Restart the stock-service container in order for StockCreator to add tables and test data.

# How to setup stored procedures

Access the database through SQL Server Management Studio:

Servername: localhost,1433  
Login: sa  
Password {YourPasswordHere}

Replace {YourPasswordHere} to your own password defined for the sa user.

Execute the query in StockDb:

```
CREATE PROCEDURE dbo.GetAllStocks
AS
BEGIN
SELECT
s.Id,
s.ProductId,
s.Quantity,
s.ReservedQuantity,
s.SoldQuantity,
p.Name,
p.Description,
p.Price
FROM Stocks s
LEFT JOIN Products p ON p.Id = s.ProductId;
END;
GO

CREATE PROCEDURE dbo.GetStockById
@Id INT
AS
BEGIN
SELECT
s.Id,
s.ProductId,
s.Quantity,
s.ReservedQuantity,
s.SoldQuantity,
p.Name,
p.Description,
p.Price
FROM Stocks s
LEFT JOIN Products p ON p.Id = s.ProductId
WHERE s.Id = @Id;
END;
GO

CREATE PROCEDURE dbo.CreateStock
(
@ProductId INT,
@Quantity INT
)
AS
BEGIN
INSERT INTO Stocks (ProductId, Quantity, ReservedQuantity, SoldQuantity)
VALUES (@ProductId, @Quantity, 0, 0);

    SELECT s.*, p.*
    FROM Stocks s
    LEFT JOIN Products p ON p.Id = s.ProductId
    WHERE s.Id = SCOPE_IDENTITY();
END;
GO

CREATE PROCEDURE dbo.UpdateStockQuantity
(
@Id INT,
@Quantity INT  
)
AS
BEGIN
UPDATE Stocks
SET Quantity = Quantity + @Quantity
WHERE Id = @Id;

    SELECT 
        s.Id,
        s.ProductId,
        s.Quantity,
        s.ReservedQuantity,
        s.SoldQuantity,
        p.Name,
        p.Description,
        p.Price
    FROM Stocks s
    LEFT JOIN Products p ON p.Id = s.ProductId
    WHERE s.Id = @Id;
END;
GO

CREATE PROCEDURE dbo.FreeProductReservation
(
    @ProductId INT,
    @Quantity INT
)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM Stocks WHERE ProductId = @ProductId)
    BEGIN
        UPDATE Stocks
        SET ReservedQuantity = ReservedQuantity - @Quantity,
            Quantity = Quantity + @Quantity
        WHERE ProductId = @ProductId;
    END
    ELSE
    BEGIN
        RAISERROR('Stock not found for the given ProductId', 16, 1);
    END
END
GO

CREATE PROCEDURE dbo.ReturnStock
(
    @ProductId INT,
    @Quantity INT
)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM Stocks WHERE ProductId = @ProductId)
    BEGIN
        UPDATE Stocks
        SET SoldQuantity = SoldQuantity - @Quantity,
            Quantity = Quantity + @Quantity
        WHERE ProductId = @ProductId;
    END
    ELSE
    BEGIN
        RAISERROR('Stock not found for the given ProductId', 16, 1);
    END
END
GO

CREATE PROCEDURE dbo.SellStock
(
    @ProductId INT,
    @Quantity INT
)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM Stocks WHERE ProductId = @ProductId)
    BEGIN
        UPDATE Stocks
        SET ReservedQuantity = ReservedQuantity - @Quantity,
            SoldQuantity = SoldQuantity + @Quantity
        WHERE ProductId = @ProductId;
    END
    ELSE
    BEGIN
        RAISERROR('Stock not found for the given ProductId', 16, 1);
    END
END
GO

CREATE PROCEDURE dbo.CancelStock
(
    @ProductId INT,
    @Quantity INT
)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM Stocks WHERE ProductId = @ProductId AND SoldQuantity >= @Quantity)
    BEGIN
        UPDATE Stocks
        SET ReservedQuantity = ReservedQuantity + @Quantity,
            SoldQuantity = SoldQuantity - @Quantity
        WHERE ProductId = @ProductId;
    END
    ELSE
    BEGIN
        RAISERROR('Cannot cancel more than sold quantity', 16, 1);
    END
END
GO

CREATE PROCEDURE dbo.ReserveStock
(
    @ProductId INT,
    @Quantity INT
)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM Stocks WHERE ProductId = @ProductId AND Quantity >= @Quantity)
    BEGIN
        UPDATE Stocks
        SET Quantity = Quantity - @Quantity,
            ReservedQuantity = ReservedQuantity + @Quantity
        WHERE ProductId = @ProductId;
    END
    ELSE
    BEGIN
        RAISERROR('Insufficient stock quantity to reserve the requested amount', 16, 1);
    END
END
GO
```

Run the following query to REVOKE and GRANT permissions to StockService:

```
REVOKE SELECT, INSERT, UPDATE, DELETE ON SCHEMA::dbo TO StockServiceRW;
GO

GRANT EXECUTE ON dbo.GetAllStocks TO StockServiceRW;
GRANT EXECUTE ON dbo.GetStockById TO StockServiceRW;
GRANT EXECUTE ON dbo.CreateStock TO StockServiceRW;
GRANT EXECUTE ON dbo.UpdateStockQuantity TO StockServiceRW;
GRANT EXECUTE ON dbo.FreeProductReservation TO StockServiceRW;
GRANT EXECUTE ON dbo.ReserveStock TO StockServiceRW;
GRANT EXECUTE ON dbo.SellStock TO StockServiceRW;
GRANT EXECUTE ON dbo.ReturnStock TO StockServiceRW;
GRANT EXECUTE ON dbo.CancelStock TO StockServiceRW;
```

# Test Stored Procedures

To test the stored procedures for GetAllStocks, FreeProductReservation, ReservesStock and SellStock, you can repeat the `Test Add Product To Order`, `Test Remove Product From Order` and `Test Place an Order` steps to validate the methods work.  

To test Get By Id:

`http://localhost:5000/api/stock/{id}` (GET)

To test Create Stock:

`http://localhost:5000/api/stock/create` (POST)

```json
{
"id": 0,
"productId": 1,
"quantity": 20
}
```

To test Update Stock:

`http://localhost:5000/api/stock/update` (PUT)

```json
{
"id": 1,
"productId": 0,
"quantity": 10
}
```

Cancel and Return stock can only be tested through the database. Login StockServiceLogin to test the procedures.

To test all the stored procedures through the database, login StockService. Example queries:

Get All Stock:
```
EXEC dbo.GetAllStocks;
```

Get Stock By Id:
```
EXEC dbo.GetStockById @Id = 1;
```

Create Stock:
```
EXEC dbo.CreateStock @ProductId = 1, @Quantity = 50;
```

Update Stock:
```
EXEC dbo.UpdateStockQuantity @Id = 1, @Quantity = 25;
```

Free Product Reservation:
``` 
EXEC dbo.FreeProductReservation @ProductId = 1, @Quantity = 10;
``` 

Return Stock:
```
EXEC dbo.ReturnStock @ProductId = 1, @Quantity = 5;
```

Sell Stock:
```
EXEC dbo.SellStock @ProductId = 1, @Quantity = 15;
```

Cancel Stock:
```
EXEC dbo.CancelStock @ProductId = 1, @Quantity = 10;
```

Reserve Stock:
```
EXEC dbo.ReserveStock @ProductId = 1, @Quantity = 20;
```

Validate StockService only can run procedures. Attempt to run any SELECT, INSERT, UPDATE and DELETE and permission should be denied.



