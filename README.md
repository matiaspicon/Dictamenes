# Dictamenes
# Información técnica del proyecto

Nombre del proyecto: Dictámenes

Tecnología utilizada: .NET Framework 4.5

Arquitectura utilizada: MVC

Motor de base de datos: SQL Server con Entity Framework y Linq

# UML Diagrama de Clases

![alt text](https://github.com/matiaspicon/Dictamenes/blob/master/Readme/UML%20de%20Clases.png?raw=true)

UML Diagrama de navegabilidad

![alt text](https://github.com/matiaspicon/Dictamenes/blob/master/Readme/Navegabilidad.png?raw=true)

# Tablas de Base de Datos


![alt text](https://github.com/matiaspicon/Dictamenes/blob/master/Readme/tablas%20de%20BD.jpg?raw=true)

# Modelos de Negocio

## Dictámenes

Atributos

**Numero de GDE:** Este campo es obligatorio y debe ser único (No puede haber dos dictámenes con el mismo Número de GDE). Este campo verifica el valor ingresado con este formato regex: _&quot;[iI][fF]-[0-9]{4}-[0-9]+-[aA][pP][nN]-[A-Za-z]+#[A-Za-z]+_&quot;.

**Número de Expediente:** Este campo es obligatorio y no es único (Puede haber más de un dictamen con el mismo Número de Expediente). Este campo verifica el valor ingresado con este formato regex: _&quot;[eE][xX]-[0-9]{4}-[0-9]+-[aA][pP][nN]-[A-Za-z]+#[A-Za-z]+&quot;_.

**Fecha de carga:** Este campo corresponde a la fecha de carga en el GDE y es obligatorio. El formato de la fecha es: &quot;26/10/2021 15:40:00&quot;.

**Archivo:** En este campo se almacena el objeto del archivo del Dictamen. Puede ser null en el caso de que se haya cargado sin archivo o se le haya borrado después.

**Detalle:** Este campo es obligatorio y sería un campo de texto para escribir un detalle del dictamen.

**Asunto:** Este campo es obligatorio y el valor viene de la tabla maestra de Asuntos.

**Tipo de Dictamen:** Este campo no es obligatorio y el valor vendría de la tabla maestra de Tipos de Dictamen.

**Sujeto Obligado:** Contiene el objeto Sujeto Obligado, puede ser &quot;null&quot;.

**Hay Sujeto Obligado** : Valor booleano que indica si el Dictamen tiene o no tiene un Sujeto Obligado.

**Id Usuario Modificación** : Contiene el ID del usuario que realizo la última modificación de este mismo objeto. Dicho ID viene del login universal.

**Fecha Modificación:** Indica el último momento cuando se realizó la modificación de este mismo objeto.

## Sujetos Obligados:

Atributos:

_**CUIL/CUIT:**_

Este campo recibe el Cuil/Cuit del Sujeto Obligado, ser válido (se realiza comprobación matemática del mismo) y también único (no puede haber dos Sujetos Obligados con el mismo Cuil/Cuit). En caso de modificar este valor, se modificará en todos los lugares donde haya sido utilizado.

_**Razón Social:**_

Este campo recibe la Razón Social de la empresa o compañía. El valor de este campo es el que se va a tomar en la lista desplegable. En caso de modificar este valor, se modificará en todos los lugares donde haya sido utilizado.

_**Nombre:**_

Este campo recibe el Nombre del Sujeto Obligado.

_**Apellido:**_

Este campo recibe el Apellido del Sujeto Obligado.

_**Está habilitado:**_

Tildar esta opción hace que el valor esté disponible para ser seleccionado en las listas desplegables correspondientes. Destildar esta opción hace que no esté disponible para ser seleccionado en las listas desplegables correspondientes, este no elimina el valor de la tabla por lo que en un futuro podría ser reactivado.

_**Tipo de Sujeto Obligado:**_

El valor de este campo viene de la tabla maestra de Tipo de Sujeto Obligado y es obligatorio.

**Id Usuario Modificación** : Contiene el ID del usuario que realizo la última modificación de este mismo objeto. Dicho ID viene del login universal.

**Fecha Modificación:** Indica el último momento cuando se realizó la modificación de este mismo objeto.

Tipo de sujetos obligados:

Particulares o denunciantes:

_**CUIL/CUIT:**_ Igual.

_**Razón Social:**_ Valor &quot;null&quot;.

_**Nombre:**_ Este campo recibe el Nombre del Sujeto Obligado.

_**Apellido:**_ Este campo recibe el Apellido del Sujeto Obligado.

_**Está habilitado:**_ Siempre en true.

_**Tipo de Sujeto Obligado:**_ El valor del Tipo de Sujeto Obligado es cualquiera de la tabla maestra de Tipo de Sujeto Obligado

Compañía:

_**CUIL/CUIT:**_ Igual.

_**Razón Social:**_ Este campo recibe la Razón Social de la empresa o compañía.

_**Nombre:**_ Valor &quot;null&quot;.

_**Apellido:**_ Valor &quot;null&quot;.

_**Está habilitado:**_ Siempre en true.

_**Tipo de Sujeto Obligado:**_ El valor del Tipo de Sujeto Obligado es siempre &quot;Denunciante&quot;.

## Campos:

_**Descripción:**_

Valor que va a tomar el campo de la tabla maestra. En caso de modificar este valor, se modificará en todos los lugares donde haya sido utilizado.

_**Está habilitado:**_

Tildar esta opción hace que el valor esté disponible para ser seleccionado en los desplegables correspondientes. Destildar esta opción hace que no esté disponible para ser seleccionado en los desplegable correspondientes, este no elimina el valor de la tabla por lo que en un futuro podría ser reactivado.

**Id Usuario Modificación** : Contiene el ID del usuario que realizo la última modificación de este mismo objeto. Dicho ID viene del login universal.

**Fecha Modificación:** Indica el último momento cuando se realizó la modificación de este mismo objeto.

_Los objetos Asunto, TipoDictamen y TipoSujetoObligado heredan del objeto Campo._


# Roles:

## Consultar:

Este rol tiene como objetivo poder consultar los dictámenes cargados en el sistema.

Tiene acceso a:

- /dictamenes/index
- /dictamenes/details

## Cargar:

Este rol tiene como objetivo poder cargar los dictámenes cargados en el sistema.

Tiene acceso a:

- /dictamenes/index
- /dictamenes/details
- /dictamenes/cargarfile
- /dictamenes/create
- /dictamenes/edit
- /dictamenes/delete
- /campos/menu
- /asuntos/index
- /asuntos/create
- /asuntos/edit
- /TiposSujetoObligado/index
- /TiposSujetoObligado/create
- /TiposSujetoObligado/edit
- /TiposDictamen/index
- /TiposDictamen/create
- /TiposDictamen/edit
- /sujetosobligados/index
- /sujetosobligados/create
- /sujetosobligados/edit
- /denunciantes/index
- /denunciantes/edit


