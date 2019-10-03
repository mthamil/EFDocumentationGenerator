This is a Visual Studio plugin for the ADO.NET Entity Designer. It hooks into the model update process in order to pull MS_Description extended properties from a SQL Server database and populate an entity model's (.edmx file) Documentation nodes with them. This makes these descriptions available for use during code generation, enabling the addition of comments to generated classes and members.

The plugin searches the project that an .edmx file belongs to for a config file containing an Entity Framework connection string so that it can connect to the database.

This tool does not require explicit invocation. When using database first with Entity Framework, simply open the Entity Designer by opening an .edmx file. Then, right click the designer and choose "Update Model from Database" and follow the instructions. If you have MS_Description extended properties on your columns and/or tables, the edmx file should be populated with these descriptions when performing Add or Refresh.  

You can verify this by either looking at the raw edmx's Conceptual Model, or in the designer, right clicking an entity or property, selecting Properties, and expanding Documentation. Currently, the Summary property is filled.

To make use of this extra information, the template (.tt) file that generates the entities will require modifications. For guidance on modifying the template accordingly, please visit the following page about [template modification](https://github.com/mthamil/EFDocumentationGenerator/wiki/Template-Modification).

This extension will fail if no App.config or Web.config containing a connection string exists in the project containing the .edmx file. Therefore, it will currently not work for initial generation, only subsequent updates (unless a valid connection string is placed in a config file beforehand).

The source code for this plugin is released under the Apache 2.0 license and is located at: [https://github.com/mthamil/EFDocumentationGenerator](https://github.com/mthamil/EFDocumentationGenerator)

**Release v2.3.0**
 - Support Visual Studio 2019.

**Release v2.2.0**
 - Support property names that differ from their mapped column names.

**Release v2.1.1**
- Fix bug where application configs with child items such as transforms were not being found when searching for a valid config. Note that the transform files themselves are still not considered.

**Release v2.1.0**
- Support tables whose names are not the same as their entity names (ie. pluralized table names).

**Release v2.0.0**
- Drop support for older Visual Studio versions and add support for Visual Studio 2017.
- Detect connection strings in web.config files.

**Release v1.3:**
- Implemented population of Navigation Property Documentation nodes from MS_Description properties on Constraints.  
- Added output pane warning for when a model contains pre-existing errors.

**Release v1.2:**
- Improved overall error handling.  
- Status messages are now written to the "Entity Documentation Generator" Output Window pane.  
- App.config files with unsaved changes will now be saved in order to try to pick up connection string changes.  
- Fixed crash when performing an initial generation.

**Release v1.1:**
- Added support for tables belonging to non-default schemas.