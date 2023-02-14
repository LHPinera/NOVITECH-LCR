# NOVITECH-LCR
### Aplicación para lectura de tags del REPUVE
### Autor: Luis Humberto Piñera Coronado
### Versión 1

Este programa recibe los datos generados por el lector RFID ImpInj, se basa en la aplicacion original del REPUVE TestImpInj y fue adaptado para el sistema de arcos moviles REPUVE

Utiliza el sdk ImpInjOctane.sdk

Su diseño es sencillo

Se iniciliza un windowsform llamada Dashboard el cual contiene TextBoxes para mostrar los datos del tag leido, los cuales son:
    Fecha
    Número de antena
    Folio del tag 
Tambien se incluye un textbox con un contador del número de tags leidos

Cuando se recibe una lectura se ejecuta la funcion OnTagsReported la cual ejecuta ShowData para mostrar los datos en pantalla y GrabaChip para insertar la lectura en la base de datos del sistema.

Funciones de Dashboard

DashBoard - Función principal
OnTagsReported - La dispara el sdk cuando se recibe una lectura
ConvertHex - Convierte una cadena ASCII en hexadecimal
ShowData - Actualiza los textboxes de la pantalla principal
Detener - Se ejecuta al hacer click en el botón Detener
Iniciar - Se ejecuta al hacer click en el botón Iniciar
GrabaChip - Se lanza desde OnTagsReported e inserta un registro en la base de datos

Hay dos clases

Lectura que incluye los datos que se reciben del tag
Chip que corresponde a la tabla Chips_leidos de la base de datos

### ACTUALIZADO A FEBRERO DE 2023
