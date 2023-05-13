CREATE OR REPLACE PACKAGE GENERAL_PACKAGE IS

------------------------------------ARTISTS-------------------------------------
  PROCEDURE GET_ARTIST_BY_NAME (I_ARTIST_NAME IN ARTISTS.ARTIST_NAME%TYPE,
                                          O_ARTIST_CURS OUT SYS_REFCURSOR);
                                          
  PROCEDURE GET_ARTIST_BY_ID (I_ARTIST_ID IN ARTISTS.ARTIST_ID%TYPE,
                                    O_ARTIST_CURS OUT SYS_REFCURSOR);
                                    
  PROCEDURE GET_ALL_ARTISTS (O_ARTIST_CURS OUT SYS_REFCURSOR);

------------------------------------SONGS--------------------------------------
  PROCEDURE GET_ALL_SONGS (O_SONG_CURS OUT SYS_REFCURSOR);
  
  PROCEDURE GET_SONG_BY_ID (I_SONG_ID IN SONGS.SONG_ID%TYPE,
                              O_SONG_CURS OUT SYS_REFCURSOR);
                              
  PROCEDURE GET_SONG_BY_ARTIST (I_ARTIST_ID IN ARTISTS.ARTIST_ID%TYPE,
                                        O_SONG_CURS OUT SYS_REFCURSOR);
                              
------------------------------------ALBUMS--------------------------------------
  PROCEDURE GET_ALL_ALBUMS (O_ALBUMS_CURS OUT SYS_REFCURSOR);
  
  PROCEDURE GET_ALBUM_BY_ID (I_ALBUM_ID IN ALBUMS.ALBUM_ID%TYPE,
                                O_ALBUMS_CURS OUT SYS_REFCURSOR);           
  
  PROCEDURE GET_SONGS_BY_ALBUM (I_ALBUM_ID IN ALBUMS.ALBUM_ID%TYPE,
                                    O_SONGS_CURS OUT SYS_REFCURSOR);
  
  PROCEDURE GET_ALBUMS_BY_SONG (I_SONG_ID IN SONGS.SONG_ID%TYPE,
                                O_ALBUMS_CURS OUT SYS_REFCURSOR); 
                                    
---------------------------------FAVORITE_SONG----------------------------------                                  
  PROCEDURE GET_AMOUNT_USERS_BY_FAVOURITES (I_SONG_ID IN SONGS.SONG_ID%TYPE,
                                                  O_USERS_AMOUNT OUT NUMBER);
  PROCEDURE GET_TOP_FAVORITE_SONGS (O_TOP_SONGS_CURS OUT SYS_REFCURSOR);

-------------------------------FOLLOWED_ARTISTS---------------------------------                                                     
  PROCEDURE GET_ARTIST_FOLLOWERS_AMOUNT (I_ARTIST_ID IN ARTISTS.ARTIST_ID%TYPE,
                                                O_FOLLOWERS_AMOUNT OUT NUMBER);
  
  PROCEDURE GET_TOP_FOLLOWED_ARTISTS (O_TOP_FOLLOWED_CURS OUT SYS_REFCURSOR);  
END; 


CREATE OR REPLACE PACKAGE BODY GENERAL_PACKAGE IS
------------------------------------ARTISTS-------------------------------------
-------------------------GET_ALL_ARTISTS------------------------
PROCEDURE GET_ALL_ARTISTS
  (O_ARTIST_CURS OUT SYS_REFCURSOR)
IS
BEGIN
  OPEN O_ARTIST_CURS FOR SELECT * FROM ARTISTS;
END GET_ALL_ARTISTS;
-------------------------GET_ARTIST_BY_NAME------------------------
PROCEDURE GET_ARTIST_BY_NAME
  (I_ARTIST_NAME IN ARTISTS.ARTIST_NAME%TYPE,
  O_ARTIST_CURS OUT SYS_REFCURSOR)
IS
BEGIN
  OPEN O_ARTIST_CURS FOR SELECT * FROM ARTISTS WHERE ARTIST_NAME = I_ARTIST_NAME;
  IF O_ARTIST_CURS%NOTFOUND THEN
    RAISE_APPLICATION_ERROR(-20017, 'Artist not found');
  END IF;
END GET_ARTIST_BY_NAME;

-------------------------GET_ARTIST_BY_ID------------------------
PROCEDURE GET_ARTIST_BY_ID
  (I_ARTIST_ID IN ARTISTS.ARTIST_ID%TYPE,
  O_ARTIST_CURS OUT SYS_REFCURSOR)
IS
BEGIN
  OPEN O_ARTIST_CURS FOR SELECT * FROM ARTISTS WHERE ARTIST_ID = I_ARTIST_ID;
  IF O_ARTIST_CURS%NOTFOUND THEN
    RAISE_APPLICATION_ERROR(-20017, 'Artist not found');
  END IF;
END GET_ARTIST_BY_ID;

------------------------------------SONGS---------------------------------------
-------------------------GET_ALL_SONGS-------------------------
PROCEDURE GET_ALL_SONGS
  (O_SONG_CURS OUT SYS_REFCURSOR)
IS
BEGIN
  OPEN O_SONG_CURS FOR SELECT * FROM SONGS ;
END GET_ALL_SONGS;
-------------------------GET_SONG_BY_ID-------------------------
PROCEDURE GET_SONG_BY_ID
  (I_SONG_ID IN SONGS.SONG_ID%TYPE,
  O_SONG_CURS OUT SYS_REFCURSOR)
IS
BEGIN
  OPEN O_SONG_CURS FOR SELECT * FROM SONGS 
    WHERE SONG_ID = I_SONG_ID;
  IF O_SONG_CURS%NOTFOUND THEN
    RAISE_APPLICATION_ERROR(-20019, 'Song not found');
  END IF;
END GET_SONG_BY_ID;

-------------------------GET_SONG_BY_ARTIST-------------------------
PROCEDURE GET_SONG_BY_ARTIST
  (I_ARTIST_ID IN ARTISTS.ARTIST_ID%TYPE,
  O_SONG_CURS OUT SYS_REFCURSOR)
IS
BEGIN
  OPEN O_SONG_CURS FOR SELECT * FROM SONGS 
    WHERE ARTIST_ID = I_ARTIST_ID;
  IF O_SONG_CURS%NOTFOUND THEN
    RAISE_APPLICATION_ERROR(-20019, 'Song not found');
  END IF;
END GET_SONG_BY_ARTIST;

------------------------------------ALBUMS--------------------------------------
-------------------------GET_ALL_ALBUMS-------------------------
PROCEDURE GET_ALL_ALBUMS
  (O_ALBUMS_CURS OUT SYS_REFCURSOR)
IS
BEGIN
  OPEN O_ALBUMS_CURS FOR SELECT * FROM ALBUMS;
END GET_ALL_ALBUMS;
-------------------------GET_ALBUM_BY_ID-------------------------
PROCEDURE GET_ALBUM_BY_ID
  (I_ALBUM_ID IN ALBUMS.ALBUM_ID%TYPE,
  O_ALBUMS_CURS OUT SYS_REFCURSOR)
IS
BEGIN
  OPEN O_ALBUMS_CURS FOR SELECT * FROM ALBUMS WHERE ALBUM_ID = I_ALBUM_ID;
  IF O_ALBUMS_CURS%NOTFOUND THEN 
    RAISE_APPLICATION_ERROR(-20018, 'Album not found');
  END IF;
END GET_ALBUM_BY_ID;

-------------------------GET_SONGS_BY_ALBUM-------------------------
PROCEDURE GET_SONGS_BY_ALBUM
  (I_ALBUM_ID IN ALBUMS.ALBUM_ID%TYPE,
  O_SONGS_CURS OUT SYS_REFCURSOR)
IS
BEGIN
    OPEN O_SONGS_CURS FOR SELECT SONG_ID, SONG_NAME, SONG_FILE, ARTIST_NAME
      FROM ARTIST_ALBUM_SONG_VIEW WHERE ALBUM_ID = I_ALBUM_ID;
    IF O_SONGS_CURS%NOTFOUND THEN
      RAISE_APPLICATION_ERROR(-20018, 'Album not found');
    END IF;
END GET_SONGS_BY_ALBUM;

-------------------------GET_ALBUMS_BY_SONG-------------------------
PROCEDURE GET_ALBUMS_BY_SONG
  (I_SONG_ID IN SONGS.SONG_ID%TYPE,
  O_ALBUMS_CURS OUT SYS_REFCURSOR)
IS
BEGIN
    OPEN O_ALBUMS_CURS FOR SELECT ALBUM_ID, ALBUM_NAME, ALBUM_RELEASE_DATE, ALBUM_COVER
      FROM ARTIST_ALBUM_SONG_VIEW WHERE SONG_ID = I_SONG_ID;
    IF O_ALBUMS_CURS%NOTFOUND THEN
      RAISE_APPLICATION_ERROR(-20019, 'Song not found');
    END IF;
END GET_ALBUMS_BY_SONG;

---------------------------------FAVORITE_SONG----------------------------------

------GET_AMOUNT_USERS_BY_FAVOURITES---------
PROCEDURE GET_AMOUNT_USERS_BY_FAVOURITES
(I_SONG_ID IN SONGS.SONG_ID%TYPE,
O_USERS_AMOUNT OUT NUMBER)
IS
BEGIN
    SELECT COUNT(USER_ID) INTO O_USERS_AMOUNT    
    FROM FAVS_USER_SONG_ARTIST_VIEW
      WHERE SONG_ID = I_SONG_ID;
END GET_AMOUNT_USERS_BY_FAVOURITES;

--------GET_TOP_FAVORITE_SONGS------
PROCEDURE GET_TOP_FAVORITE_SONGS
(O_TOP_SONGS_CURS OUT SYS_REFCURSOR)
IS
BEGIN
  OPEN O_TOP_SONGS_CURS FOR
    SELECT * FROM TOP_FAVOURITE_SONGS_VIEW;
END GET_TOP_FAVORITE_SONGS;

-------------------------------FOLLOWED_ARTISTS---------------------------------

------GET_ARTIST_FOLLOWERS_AMOUNT----------
PROCEDURE GET_ARTIST_FOLLOWERS_AMOUNT
  (I_ARTIST_ID IN ARTISTS.ARTIST_ID%TYPE,
  O_FOLLOWERS_AMOUNT OUT NUMBER)
  IS
  BEGIN
      SELECT COUNT(*) INTO O_FOLLOWERS_AMOUNT 
      FROM FOLLOWED_ARTISTS 
      WHERE ARTIST_ID = I_ARTIST_ID;
  END GET_ARTIST_FOLLOWERS_AMOUNT;
  
--------------GET_TOP_FOLLOWED_ARTISTS---------------
PROCEDURE GET_TOP_FOLLOWED_ARTISTS
(O_TOP_FOLLOWED_CURS OUT SYS_REFCURSOR)
IS
BEGIN
  OPEN O_TOP_FOLLOWED_CURS FOR 
  SELECT ARTIST_ID FROM TOP_FOLLOWED_ARTISTS_VIEW;
END GET_TOP_FOLLOWED_ARTISTS;

END;