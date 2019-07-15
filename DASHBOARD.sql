-- VISTAS PARA DASHBOARD --



  
  CREATE VIEW VISTA_MAS_VENDIDOS AS
  SELECT D.ID_MERCADERIA, M.DESCRIPCION, SUM(D.CANTIDAD_DET_FACTURA), D.ID_SUCURSAL FROM DET_FACTURA D
  INNER JOIN MERCADERIA M 
  ON D.ID_MERCADERIA = M.ID_MERCADERIA AND D.ESTADO_TUPLA = TRUE AND M.ESTADO_TUPLA = TRUE
  GROUP BY D.ID_MERCADERIA 
  ORDER BY COUNT(D.CANTIDAD_DET_FACTURA) DESC LIMIT 10;

  
  
   CREATE VIEW VISTA_MAS_VENDIDOS_MES AS
  SELECT D.ID_MERCADERIA, M.DESCRIPCION, SUM(D.CANTIDAD_DET_FACTURA),(SELECT FECHA_EMISION_FACTURA FROM FACTURA WHERE ID_FACTURA = D.ID_FACTURA) AS FECHA FROM DET_FACTURA D
  INNER JOIN MERCADERIA M 
  ON D.ID_MERCADERIA = M.ID_MERCADERIA AND D.ESTADO_TUPLA = TRUE AND M.ESTADO_TUPLA = TRUE
  GROUP BY D.ID_MERCADERIA 
  ORDER BY COUNT(D.CANTIDAD_DET_FACTURA) DESC LIMIT 10;
  

  
  SELECT COUNT(ID_FACTURA) FROM FACTURA WHERE ESTADO_TUPLA = TRUE AND FECHA_EMISION_FACTURA >= curdate();
  
  SELECT SUM(TOTAL_FACTURA) FROM FACTURA WHERE ESTADO_TUPLA = TRUE AND ESTADO_FACTURA = 1 AND TIPO_FACTURA = "EFECTIVO" AND FECHA_EMISION_FACTURA >= curdate();
  
  SELECT SUM(TOTAL_FACTURA) FROM FACTURA WHERE ESTADO_TUPLA = TRUE AND ESTADO_FACTURA = 1 AND TIPO_FACTURA = "CREDITO" AND FECHA_EMISION_FACTURA >= curdate();

CREATE VIEW GANANCIAS
AS
 SELECT SUM(D.CANTIDAD_DET_FACTURA * M.PRECIO_1) FROM DET_FACTURA D
 INNER JOIN MERCADERIA_PRESENTACIONES M 
  ON M.ID_STOCK = D.ID_STOCK AND M.ID_SUCURSAL = D.ID_SUCURSAL
 INNER JOIN FACTURA F 
 ON D.ID_FACTURA = F.ID_FACTURA AND F.ESTADO_TUPLA = TRUE AND F.ESTADO_FACTURA = 1 AND F.FECHA_EMISION_FACTURA >= curdate();


 SELECT COUNT(ID_FACTURA) FROM FACTURA WHERE ESTADO_TUPLA = TRUE AND ESTADO_FACTURA = 1 AND FECHA_EMISION_FACTURA >= CURDATE() AND TIPO_FACTURA = 'EFECTIVO';

 SELECT COUNT(ID_CLIENTE) FROM CLIENTE WHERE FECHA_INGRESO >= CURDATE() AND ESTADO_TUPLA = TRUE;
 
 SELECT COUNT(ID_PROVEEDOR) FROM PROVEEDOR WHERE FECHA_INGRESO >= CURDATE() AND ESTADO_TUPLA = TRUE;
 
 SELECT COUNT( distinct ID_MERCADERIA) FROM MERCADERIA WHERE ESTADO_TUPLA = TRUE;


 CREATE VIEW VISTA_GANANCIA AS
 SELECT MONTHNAME(F.FECHA_EMISION_FACTURA) AS MES, SUM(F.TOTAL_FACTURA) AS TOTAL FROM FACTURA F WHERE(YEAR(F.FECHA_EMISION_FACTURA) = YEAR(NOW()) AND F.ESTADO_TUPLA = TRUE AND F.ESTADO_FACTURA = 1) 
 GROUP BY MES ;
 

 
 
 
 CREATE VIEW VISTA_GANANCIA_PERIODO AS
 SELECT MONTHNAME(F.FECHA_EMISION_FACTURA) AS MES, truncate( SUM(D.CANTIDAD_DET_FACTURA * M.PRECIO_1),2) AS PRECIO_NETO,  truncate (SUM(D.SUBTOTAL_DETALLE * D.CANTIDAD_DET_FACTURA),2) AS VALOR_VENDIDO,( truncate (SUM(D.SUBTOTAL_DETALLE * D.CANTIDAD_DET_FACTURA)- SUM(D.CANTIDAD_DET_FACTURA * M.PRECIO_1),2)) AS GANANCIA FROM DET_FACTURA D
 INNER JOIN MERCADERIA_PRESENTACIONES M 
  ON M.ID_STOCK = D.ID_STOCK AND M.ID_SUCURSAL = D.ID_SUCURSAL
 INNER JOIN FACTURA F 
 ON D.ID_FACTURA = F.ID_FACTURA AND F.ESTADO_TUPLA = TRUE AND F.ESTADO_FACTURA = 1
 GROUP BY MES;




 CREATE VIEW VISTA_GANANCIA_DIAS AS
 SELECT F.FECHA_EMISION_FACTURA AS MES, SUM(D.CANTIDAD_DET_FACTURA * M.PRECIO_1), SUM(D.SUBTOTAL_DETALLE * D.CANTIDAD_DET_FACTURA) FROM DET_FACTURA D
 INNER JOIN MERCADERIA_PRESENTACIONES M 
  ON M.ID_STOCK = D.ID_STOCK AND M.ID_SUCURSAL = D.ID_SUCURSAL
 INNER JOIN FACTURA F 
 ON D.ID_FACTURA = F.ID_FACTURA AND F.ESTADO_TUPLA = TRUE AND F.ESTADO_FACTURA = 1
 GROUP BY MES;
 
  CREATE VIEW 
  TODAS_SUCURSALES AS
   SELECT M.ID_MERCADERIA, M.DESCRIPCION,M.PRECIO_COMPRA,(SELECT SUM(TOTAL_UNIDADES) FROM MERCADERIA WHERE ID_MERCADERIA = M.ID_MERCADERIA) ,
   (SELECT NOMBRE_PROVEEDOR FROM PROVEEDOR WHERE ID_PROVEEDOR = M.ID_PROVEEDOR)
   FROM MERCADERIA M WHERE M.ACTIVO = TRUE AND M.ESTADO_TUPLA = TRUE GROUP BY M.ID_MERCADERIA;
   
 
 
 CREATE VIEW VISTA_PRECIOS_MERCADERIA AS
  SELECT DISTINCT D.ID_STOCK,M.DESCRIPCION, D.DESCRIPCION AS TIPO_VENTA, D.CANTIDAD_POR_PRESENTACION,
  D.TOTAL_PRESENTACIONES,D.PRECIO_2, D.PRECIO_3, D.PRECIO_4,
  D.PRECIO_5,D.PRECIO_6, S.NOMBRE_SUCURSAL FROM MERCADERIA_PRESENTACIONES D
  INNER JOIN MERCADERIA M 
  ON D.ID_MERCADERIA = M.ID_MERCADERIA
  INNER JOIN SUCURSAL S
  ON D.ID_SUCURSAL = S.ID_SUCURSAL;
  

  
   CREATE VIEW VISTA_PRECIOS_MERCADERIA AS
  SELECT DISTINCT D.ID_STOCK, CONCAT (M.DESCRIPCION,' - ', D.DESCRIPCION,' (' , D.CANTIDAD_POR_PRESENTACION, ')') AS DESCRIPCION,
  D.TOTAL_PRESENTACIONES,D.PRECIO_2, D.PRECIO_3, D.PRECIO_4,
  D.PRECIO_5,D.PRECIO_6, S.NOMBRE_SUCURSAL FROM MERCADERIA_PRESENTACIONES D
  INNER JOIN MERCADERIA M 
  ON D.ID_MERCADERIA = M.ID_MERCADERIA AND M.ESTADO_TUPLA = TRUE AND M.ACTIVO = TRUE 
  INNER JOIN SUCURSAL S
  ON D.ID_SUCURSAL = S.ID_SUCURSAL;
  

  
  

     CREATE VIEW INVERSION_INVENTARIO AS
  SELECT DISTINCT D.ID_STOCK, CONCAT (M.DESCRIPCION,' - ', D.DESCRIPCION,' (',D.CANTIDAD_POR_PRESENTACION,')') AS DESCRIPCION,
  D.PRECIO_1 AS PRECIO_COSTO,D.PRECIO_2 AS PRECIO ,D.TOTAL_PRESENTACIONES AS EN_ALMACEN,(D.TOTAL_PRESENTACIONES * D.PRECIO_1) AS VALOR_INVENTARIO,
  (D.TOTAL_PRESENTACIONES * D.PRECIO_2) AS VALOR_VENTA,S.NOMBRE_SUCURSAL FROM MERCADERIA_PRESENTACIONES D
  INNER JOIN MERCADERIA M 
  ON D.ID_MERCADERIA = M.ID_MERCADERIA AND M.ESTADO_TUPLA = TRUE AND M.ACTIVO = TRUE
  INNER JOIN SUCURSAL S
  ON D.ID_SUCURSAL = S.ID_SUCURSAL;

  
 /* CREATE VIEW CUENTAS_COBRAR AS 
  SELECT S.ID_CLIENTE,CONCAT (S.NOMBRE_CLIENTE,' ', S.APELLIDO_CLIENTE) AS NOMBRE_CLIENTE,C.FECHA_PAGO ,
  (SELECT SUM(TOTAL_FACTURA) FROM FACTURA WHERE ID_FACTURA = F.ID_FACTURA AND ID_CLIENTE = S.ID_CLIENTE) AS TOTAL_ADEUDADO,C.TOTAL_PAGADO,
  ((SELECT SUM(TOTAL_FACTURA) FROM FACTURA WHERE ID_FACTURA = F.ID_FACTURA)- TOTAL_PAGADO) AS BALANCE_CLIENTE FROM CLIENTE S
  INNER JOIN CUENTA_POR_COBRAR_CLIENTE C 
  ON S.ID_CLIENTE = C.ID_CLIENTE AND C.ESTADO_TUPLA = TRUE
  INNER JOIN FACTURA F 
  ON C.ID_FACTURA = F.ID_FACTURA = F.ESTADO_TUPLA = TRUE
  GROUP BY S.ID_CLIENTE;*/


  
 
  
 

  CREATE VIEW CUENTAS_COBRAR AS
  SELECT C.ID_CLIENTE, CONCAT(C.NOMBRE_CLIENTE,' ', C.APELLIDO_CLIENTE) AS NOMBRE_CLIENTE,
  D.FECHA_PAGO,(SELECT SUM(F.TOTAL_FACTURA) FROM FACTURA F WHERE F.ID_FACTURA = (SELECT ID_FACTURA FROM CUENTA_POR_COBRAR_CLIENTE WHERE ID_FACTURA = F.ID_FACTURA AND ID_CLIENTE = C.ID_CLIENTE )) AS TOTAL_ADEUDADO
  ,D.TOTAL_PAGADO, 
  ((SELECT SUM(F.TOTAL_FACTURA) FROM FACTURA F WHERE F.ID_FACTURA = (SELECT ID_FACTURA FROM CUENTA_POR_COBRAR_CLIENTE WHERE ID_FACTURA = F.ID_FACTURA AND ID_CLIENTE = C.ID_CLIENTE )) - D.TOTAL_PAGADO) AS BALANCE_CLIENTE FROM CLIENTE C
  INNER JOIN CUENTA_POR_COBRAR_CLIENTE D 
  ON  C.ID_CLIENTE = D.ID_CLIENTE AND D.ESTADO_TUPLA = TRUE
  GROUP BY C.ID_CLIENTE;
  
   CREATE VIEW CUENTAS_PAGAR AS
  SELECT C.ID_PROVEEDOR, C.NOMBRE_PROVEEDOR,
  D.FECHA_PAGO,(SELECT SUM(F.TOTAL_FINAL) FROM COMPRA F WHERE F.ID_COMPRA = (SELECT ID_COMPRA FROM CUENTA_POR_PAGAR WHERE ID_COMPRA = F.ID_COMPRA AND ID_PROVEEDOR = C.ID_PROVEEDOR )) AS TOTAL_ADEUDADO
  ,D.TOTAL_PAGADO, 
  ((SELECT SUM(F.TOTAL_FINAL) FROM COMPRA F WHERE F.ID_COMPRA = (SELECT ID_COMPRA FROM CUENTA_POR_PAGAR WHERE ID_COMPRA = F.ID_COMPRA AND ID_PROVEEDOR = C.ID_PROVEEDOR )) - D.TOTAL_PAGADO) AS BALANCE_PROVEEDOR FROM PROVEEDOR C
  INNER JOIN CUENTA_POR_PAGAR D 
  ON  C.ID_PROVEEDOR = D.ID_PROVEEDOR AND D.ESTADO_TUPLA = TRUE
  GROUP BY C.ID_PROVEEDOR;
  
  CREATE VIEW -- VISTA CLIENTES DATAGRIDVIEW ----
VISTA_CLIENTES_REPORTE AS
SELECT C.ID_CLIENTE, CONCAT(C.NOMBRE_CLIENTE,' ', C.APELLIDO_CLIENTE) AS NOMBRE_CLIENTE,C.DIRECCION_CLIENTE,C.TELEFONO_CLIENTE,
 T.TIPO, C.LIMITE_CREDITO, C.CREDITO_DISPONIBLE, C.DIAS_CREDITO, C.FECHA_INGRESO FROM CLIENTE C
INNER JOIN TIPO_CLIENTE T
ON C.ID_TIPO_CLIENTE = T.ID_TIPO_CLIENTE AND C.ESTADO_TUPLA = TRUE;  




SELECT ID_SUCURSAL, TOTAL_UNIDADES FROM MERCADERIA WHERE ID_MERCADERIA = '6SZX6AJDLR0GT' AND ESTADO_TUPLA = TRUE AND ID_SUCURSAL <> (SELECT ID_SUCURSAL FROM SUCURSAL WHERE NOMBRE_SUCURSAL = 'SANTO TOMAS - I');


   

 



 
  
  


  
  
  
  
 
 
