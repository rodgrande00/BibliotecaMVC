-- =============================================================
-- BibliotecaMVC - Base de datos y tabla de Categorias
-- Semana 8: ADO.NET + SQL Server
--
-- Ejecutar contra el servidor SQL Server antes de correr la
-- aplicacion. Es idempotente: se puede ejecutar varias veces.
-- =============================================================

IF DB_ID('BibliotecaDb') IS NULL
    CREATE DATABASE BibliotecaDb;
GO

USE BibliotecaDb;
GO

IF OBJECT_ID('dbo.Categorias', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Categorias
    (
        Id          INT            IDENTITY(1,1) NOT NULL,
        Nombre      NVARCHAR(100)  NOT NULL,
        Descripcion NVARCHAR(400)  NOT NULL,
        CONSTRAINT PK_Categorias PRIMARY KEY (Id)
    );
END
GO

-- Datos iniciales para poder probar Mostrar, Editar y Eliminar.
IF NOT EXISTS (SELECT 1 FROM dbo.Categorias)
BEGIN
    INSERT INTO dbo.Categorias (Nombre, Descripcion) VALUES
        (N'Novela',      N'Obras narrativas de ficcion en prosa.'),
        (N'Poesia',      N'Composiciones en verso y obras liricas.'),
        (N'Historia',    N'Libros sobre hechos y periodos historicos.'),
        (N'Tecnologia',  N'Programacion, redes, hardware y sistemas.'),
        (N'Infantil',    N'Cuentos y libros ilustrados para ninos.');
END
GO
