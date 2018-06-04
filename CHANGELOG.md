# Changelog


## v2.2.0 (Unreleased)

 - Support property names that differ from their mapped column names.


## v2.1.1

 - Fix bug where application configs with child items such as transforms were not being found when searching for a valid config. 
   Note that the transform files themselves are not considered.


## v2.1.0

 - Support pluralized table names.


## v2.0.0

 - Support Visual Studio 2017.
 - Discover connection strings in web.config files.


## v1.3.0

 - Implemented population of Navigation Property Documentation nodes from MS_Description properties on Constraints.
 - Added warning for when a model contains pre-existing errors.


## v1.2.0

 - Improved overall error handling.
 - Status messages are now written to the "Entity Documentation Generator" Output Window pane.
 - App.config files with unsaved changes will now be saved in order to try to pick up connection string changes.
 - Fixed crash when performing an initial generation.


## v1.1.0

 - Added support for tables belonging to schemas other than the default, 'dbo'.
