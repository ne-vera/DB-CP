--xml export

SELECT file_name FROM dba_data_files; 
CREATE OR REPLACE DIRECTORY EXPORT_DATA AS 'C:/oracle_db/xml/export';
CREATE OR REPLACE DIRECTORY IMPORT_DATA AS 'C:/oracle_db/xml/import';

CREATE OR REPLACE PROCEDURE export_users_to_xml
IS
    doc DBMS_XMLDOM.DOMDocument;
    xdata XMLTYPE;
    CURSOR xmlcur IS
        SELECT XMLELEMENT(
            "USERS",
            XMLAttributes('http://www.w3.org/2001/XMLSchema' AS "xmlns:xsi",
            'http://www.oracle.com/Users.xsd' AS "xsi:nonamespaceSchemaLocation"),
            XMLAGG(XMLELEMENT("User",
            XMLELEMENT("login", USER.USER_LOGIN),
            xmlelement("password", USER.USER_PASSWORD),
        ))) from Character;
BEGIN
open xmlcur;
    LOOP
        FETCH xmlcur INTO xdata;
        EXIT WHEN xmlcur%notfound;
    END LOOP;
    CLOSE xmlcur;
    doc := DBMS_XMLDOM.NewDOMDocument(xdata);
    DBMS_XMLDOM.WRITETOFILE(doc, 'EXPORT_DATA/characters.xml');
END;


CREATE OR REPLACE PROCEDURE import_USERS_from_xml
IS
    L_CLOB CLOB;
    L_BFILE BFILE := BFILENAME('IMPORT_DATA', 'characters.xml');
    L_DEST_OFFSET INTEGER := 1;
    L_SRC_OFFSET INTEGER := 1;
    L_BFILE_CSID NUMBER := 0;
    L_LANG_CONTEXT INTEGER := 0;
    L_WARNING INTEGER := 0;
    P DBMS_XMLPARSER.PARSER;
    V_DOC DBMS_XMLDOM.DOMDOCUMENT;
    V_ROOT_ELEMENT DBMS_XMLDOM.DOMELEMENT;
    V_CHILD_NODES DBMS_XMLDOM.DOMNODELIST;
    V_CURRENT_NODE DBMS_XMLDOM.DOMNODE;
    ch Administrator.Character%ROWTYPE;
BEGIN
    DBMS_LOB.CREATETEMPORARY (L_CLOB, TRUE);
    DBMS_LOB.FILEOPEN(L_BFILE, DBMS_LOB.FILE_READONLY);
    DBMS_LOB.LOADCLOBFROMFILE(DEST_LOB => L_CLOB, SRC_BFILE => L_BFILE, AMOUNT => DBMS_LOB.LOBMAXSIZE,
        DEST_OFFSET => L_DEST_OFFSET, SRC_OFFSET => L_SRC_OFFSET, BFILE_CSID => L_BFILE_CSID,
        LANG_CONTEXT => L_LANG_CONTEXT, WARNING => L_WARNING);
    DBMS_LOB.FILECLOSE(L_BFILE);
    COMMIT;
    
    P := DBMS_XMLPARSER.NEWPARSER;
    DBMS_XMLPARSER.PARSECLOB(P, L_CLOB);
    V_DOC := DBMS_XMLPARSER.GETDOCUMENT(P);
    V_ROOT_ELEMENT := DBMS_XMLDOM.Getdocumentelement(V_DOC);
    V_CHILD_NODES := DBMS_XMLDOM.GETCHILDRENBYTAGNAME(V_ROOT_ELEMENT, '*');
    
    FOR i IN 0 .. DBMS_XMLDOM.GETLENGTH(V_CHILD_NODES) - 1 LOOP
        V_CURRENT_NODE := DBMS_XMLDOM.ITEM(V_CHILD_NODES, i);
        
        DBMS_XSLPROCESSOR.VALUEOF(V_CURRENT_NODE,
            'login/text()', ch.login);
        DBMS_XSLPROCESSOR.VALUEOF(V_CURRENT_NODE,
            'PASSWORD/text()', ch.USER_PASSWORD);
       
        INSERT INTO CP_ADMIN.USERS (USER_LOGIN, USER_PASSWORD)
            VALUES (ch.login, ch.password);
    END LOOP;
    
    DBMS_LOB.FREETEMPORARY(L_CLOB);
    DBMS_XMLPARSER.FREEPARSER(P);
    DBMS_XMLDOM.FREEDOCUMENT(V_DOC);
    COMMIT;
END;

BEGIN
    import_users_from_xml();
end;