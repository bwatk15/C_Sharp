C_Sharp
=======

Used Visual C# to create these applications that read from / edited microsoft access databases.


==============
updateCustomer
==============
This program read from a database containing customers with a unique ID of "customer number" which 
are loaded into a combo box for selection when the application is started. When a selection is made the 
customer's information is displayed.

When adding a customer, hitting the add button will clear the fields and allow them to be updated. The add
button now reads as "Insert customer" when all information is entered and this is pressed it will calculate
the proper unique ID and insert into the database. The customer numbers combo box is updated upon insertion.

============
ordersViewer
============
This program reads from an MS access database which has multiple tables including customers and orders.
The customers table holds the customer's ID and his information. The orders table includes the information about
the order including the customer id who of created the order, when the order occurred, its order number and when
it was shipped.

The program allows the user to select a specific customer's ID and then it displays the customer's information
next to a data grid view of his/her purchases and information about those purchases.
