Entity Designer Documentation Generator
========================

This is a Visual Studio plugin for the ADO.NET Entity Designer. It hooks into the model update process
in order to pull MS_Description extended properties from a SQL Server database and populate an entity 
model's (.edmx file) Documentation nodes with them. This makes these descriptions available for use 
during code generation, enabling the addition of comments to generated classes and members.

The plugin searches the project an .edmx file belongs to for an App.config containing an Entity
Framework connection string so that it can connect to the database.
