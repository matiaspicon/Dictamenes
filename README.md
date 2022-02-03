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

Comprobación del Cuil/Cuit:

![](RackMultipart20220203-4-7okflo_html_bc1440440bd41232.png)

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

## Objetos con terminación &quot;Log&quot;:

Estos objetos son los utilizados en las tablas Log para hacer el registro de todos los cambios realizados. Para ellos cuentan con un campo extra llamado &quot;IdOriginal&quot; que hace referencia al objeto que es en la tabla principal. Cada archivo Log cuenta con la fechaModificacion y idUsuarioModificacion del momento y del usuario que realizo los cambios que se encuentran en el mismo registro. Este comportamiento se repite también en el objeto en la tabla principal que guarda esos dos datos junto a la información actual del objeto.

Ejemplo de funcionamiento:

Para el ejemplo utilizaremos la tabla TiposDictamen y TiposDictamenLog:

![Shape1](RackMultipart20220203-4-7okflo_html_a7b8790aab850f68.gif) ![](RackMultipart20220203-4-7okflo_html_21967073705f9577.png)

Estado inicial de la tabla TiposDictamen (tabla principal):

![](RackMultipart20220203-4-7okflo_html_7e50bc2cfa56ec05.png)

Se puede observar que hay objetos cargados por el usuario con ID &quot;7777&quot; el día 3-11-2021 al mediodía.

Estado inicial de la tabla TiposDictamenLog (tabla log):

![](RackMultipart20220203-4-7okflo_html_a916131824af726c.png)

Se puede observar que no se registró ningún cambio de los objetos de la tabla principal.

Ahora vamos a realizar un cambio sobre el objeto con ID 2, que sería el &quot;DICJU&quot;:

![Shape2](RackMultipart20220203-4-7okflo_html_9f754a36414e02b8.gif) ![](RackMultipart20220203-4-7okflo_html_19f69b89fbaada6e.png)

Vamos a modificar el valor de &quot;Esta habilitado&quot;:

![Shape3](RackMultipart20220203-4-7okflo_html_9f754a36414e02b8.gif) ![](RackMultipart20220203-4-7okflo_html_ff59e6fb4c7f79ee.png)

El cambio se efectuó:

![](RackMultipart20220203-4-7okflo_html_25cba963da935ada.png)

Y las bases de datos quedaron de esta manera:

Estado final de la tabla TiposDictamen:

![Shape4](RackMultipart20220203-4-7okflo_html_31fb8df926948e2e.gif) ![](RackMultipart20220203-4-7okflo_html_961e18d31acc0080.png)

Podemos observar que el valor se actualizo, &quot;EstaHabilitado&quot; paso de 1 a 0, pero también cambiaron los valores de &quot;FechaModificacion&quot; y &quot;IdUsuarioModificacion&quot; a la fecha de hoy (17-11-2021 al casi mediodía) y al usuario logeado en este momento.

Estado final de la tabla TiposDictamenLog:

![Shape5](RackMultipart20220203-4-7okflo_html_31fb8df926948e2e.gif) ![](RackMultipart20220203-4-7okflo_html_2f1be7d3f23f8968.png)

Estado inicial de la tabla principal para comparar:

![Shape6](RackMultipart20220203-4-7okflo_html_31fb8df926948e2e.gif) ![](RackMultipart20220203-4-7okflo_html_7e50bc2cfa56ec05.png)

Acá podemos observar que se agregó un registro con toda la información antigua del objeto que modificamos, pero con el agregado del &quot;IdOriginal&quot; que corresponde al id que tiene en la tabla principal.

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

# Comportamientos específicos por ruta:

## /\*:

En todas las rutas se verifican el rol para permitir el acceso o no. El rol se extrae de la cookie del usuario.

![](RackMultipart20220203-4-7okflo_html_c20196a532052cc6.png)

En caso de que no se cuente con el permiso necesario para acceder a alguna vista se redirecciona a esta vista:

![](RackMultipart20220203-4-7okflo_html_86a299741016d5f.png)

## /dictamenes/index

Esta vista hace una consulta a través de Linq que devuelve todos los dictámenes que hayan sido cargados el año actual. Este filtro no responde a la ultimaFechaModificacion sino que a la FechaCarga.

![](RackMultipart20220203-4-7okflo_html_1161a131de36b817.png)

## /dictamenes/cargarfile - POST

Este endpoint recibe el archivo del Dictamen, lo procesa para sacarle la información, lo guarda y te redirecciona a la carga de los demás datos del Dictamen.

El procesamiento lo hace en dos partes:

Primero extrae todo el texto con la función ExtractTextFromPdf dentro del FileController que recibe la dirección relativa de donde se encuentra el archivo y devuelve un string con toda el texto del PDF:

![](RackMultipart20220203-4-7okflo_html_4ec6dfdb8e8665ba.png)

En la segunda parte del procesamiento se intenta extraer el Número de GDE, el Número de Expediente y la Fecha de Carga del contenido del archivo previamente cargado. Para eso se usa la función ExtratDictamenFromString dentro del DictamenesController que recibe el contenido del Archivo en forma de string y devuelve un objeto Dictamen con la información que encontró rellenada en los campos correspondientes:

![](RackMultipart20220203-4-7okflo_html_762f6e81a1fba4ad.png)

Luego el controlador hace un return de la vista de Create con el objeto dictamen resultante del procesamiento. ![](RackMultipart20220203-4-7okflo_html_351a5db743092735.png)

## /dictamenes/create - POST

Para crear el dictamen primero se comprueba que no exista un dictamen con el mismo Número de GDE: ![](RackMultipart20220203-4-7okflo_html_5b6e6d5fa03e6cad.png)

Luego limpio y estandarizo la informacion de algunos campos para mayor homogenidad en toda la aplicación:

![](RackMultipart20220203-4-7okflo_html_e55016fabc50111d.png)

Después ejecuto el bloque de código para agregar el Sujeto Obligado:

![](RackMultipart20220203-4-7okflo_html_9ea3b95909848b45.png)

## /dictamenes/buscar – POST

Este endpoint recibe los parámetros de búsqueda desde el buscador de &quot;Búsqueda avanzada&quot;:

![](RackMultipart20220203-4-7okflo_html_c9ebe649672a9aa6.png)

Se ejecuta un Store Procedure con los parámetros recibidos:

![](RackMultipart20220203-4-7okflo_html_bd90e9e19db6d0e0.png)

Store Procedure:

![](RackMultipart20220203-4-7okflo_html_e8f7a0417666810a.png)

La lista de dictámenes se pasa a la vista &quot;Index&quot; de &quot;Dictamenes&quot; con los valores de la búsqueda realizada.

## /\*/edit – POST

Para la edición de objetos dentro de la aplicación se utiliza el mismo proceso, en este caso representado con la entidad Asunto:

1. Guarda el objeto sin actualizar en una variable aparte, llamado nombre de entidad + &quot;Viejo&quot;. En este caso es nombrado (nombre de la entidad = &quot;asunto&quot; + &quot;Viejo&quot;). El ID de que objeto debo traer de la base de datos viene del objeto que viene por parámetro con la información que hay que modificar.

![](RackMultipart20220203-4-7okflo_html_f02298f040b4d7af.png)

1. Luego con la información del objeto sin actualizar (este caso &quot;asuntoViejo&quot;) se crea un objeto LOG de la misma entidad (en este caso &quot;AsuntoLog&quot;).

![](RackMultipart20220203-4-7okflo_html_3fa90c8839d7d6d3.png)

1. A continuación, actualizo el idUsuarioModificacion y la FechaModificacion del objeto &quot;asunto&quot; que vino por parámetro con la información modificada.

![](RackMultipart20220203-4-7okflo_html_e884409e250fad01.png)

1. Después se modifica el registro en la tabla principal de esa entidad de la base de datos (en este caso la tabla seria &quot;Asuntos&quot;) y se agrega el objeto LOG a la tabla LOG de la base de datos (en este caso la tabla seria &quot;AsuntosLog&quot;). ![](RackMultipart20220203-4-7okflo_html_8283acf67921a8f1.png)

La consecuencia de este proceso es que hay un registro de todas las modificaciones hechas sobre todos los objetos de la aplicación: &quot;AsuntosLog&quot; y hay una tabla única con las últimas versiones de los objetos: &quot;Asuntos&quot;.

# Claves del Web.Config

## connectionStrings

![](RackMultipart20220203-4-7okflo_html_fba802cd4b29e9d1.png)

Se debe dejar solo uno descomentado, según en qué ambiente se encuentre la aplicación.

## Authentication

Valores:

- name
  - Uso: Nombre de la Cookie de Login
  - Valor: &quot;.DICTAMENES&quot;
- loginUrl
  - Uso: URL donde se va a redireccionar para loguear
  - Valor: &quot;login/login&quot;
- timeout
  - Uso: Tiempo en minutos de duración de sesión
  - Valor: 30

En formato RAW:

\&lt;authentication mode=&quot;Forms&quot;\&gt;

\&lt;forms name=&quot;.DICTAMENES&quot; loginUrl=&quot;login/login&quot; timeout=&quot;30&quot; /\&gt;

\&lt;/authentication\&gt;

# Implementación de Login Universal

La implementación del Login Universal está hecha con una adaptación de la librería FrameworkSSN. Se tuvo que realizar la adaptación ya que la original no es compatible con el patrón MVC, para consultas de dicha librería puede consultar la documentación de la misma.

## Framework.config:

Es el archivo que configura la librería FrameworkMVC:

![](RackMultipart20220203-4-19xg1h4_html_cd8c6f43dffa4842.png)

El archivo cuenta con los valores duplicados, un campo para Desarrollo y otro para Producción.

Campos:

- AppID: ID de la aplicación que nos asigna el Registro de Aplicaciones.
- Ejemplo: &quot;\&lt;AppID\&gt; **999** \&lt;/AppID\&gt;&quot;
- LoginUrl: URL de la dirección de donde se encuentre el Login Universal. El valor debe estar formateado para recibir como primer parámetro el ID de la aplicación y el segundo un 1 o un 0 si se encuentra local o en el servidor.
- Roles: Es una lista de roles.
  - Rol: recibe el nombre del Rol y el ID del mismo. El nombre del Rol es utilizado luego dentro en la aplicación, por lo que se recomienda crear un ENUM del mismo.
  - Ejemplo: &quot;\&lt;add Name=&quot; **ROL**&quot; Id=&quot; **999**&quot; /\&gt;&quot;
- WebReferences: Es una lista de los webReference que utiliza la aplicación.
  - LoginUniversal: Esta webReference es la que se utiliza para consultar los datos de sesión de los usuarios.

Este archivo debe ubicarse en la raíz del proyecto (en el mismo lugar que el Web.config):

Para que Framework.config sea leído por el sistema debemos incluir unas líneas en el Web.config:

_**\&lt;configSections\&gt;**_

…

_ **\&lt;section name=&quot;framework&quot; type=&quot;FrameworkMVC.Configuration.FrameworkSection, FrameworkMVC&quot; /\&gt;** _

…

_ **\&lt;/configSections\&gt;** _

_ **\&lt;framework configSource=&quot;Framework.config&quot; /\&gt;** _

## Login:

El proceso de login se realiza en el LoginController en el método Login:

![](RackMultipart20220203-4-7okflo_html_40001c9f3beb19e6.png)

Dicho endpoint es el que recibe el callback del Login Universal con el ID de sesión del usuario que se logueo, por lo que en la base de datos de aplicaciones debe ponerse este endpoint.

## Logout:

Para cerrar sesión dentro de la aplicación se debe ingresar a este método:

![](RackMultipart20220203-4-7okflo_html_d1ac7b48c7a379da.png)
