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
- **URL:** `/vault`  
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
- **URL:** `/api/order/place-order/{orderId}`  
- **Method:** `GET`  
- **Description:** Retrieves order status to confirm order has been placed.

Response:

200 OK - Returns order status

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


