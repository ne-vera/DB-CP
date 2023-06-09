CREATE OR REPLACE PACKAGE CLIENT_PACKAGE IS
  
-------------------------------------USERS--------------------------------------
  PROCEDURE REGISTER_USER (I_USER_LOGIN IN USERS.USER_LOGIN%TYPE, 
                      I_USER_PASSWORD IN USERS.USER_PASSWORD%TYPE);
                      
  PROCEDURE LOG_IN_USER (I_USER_LOGIN IN USERS.USER_LOGIN%TYPE, 
                    I_USER_PASSWORD IN USERS.USER_PASSWORD%TYPE,
                    O_USER_ID OUT USERS.USER_ID%TYPE,
                    O_USER_LOGIN OUT USERS.USER_LOGIN%TYPE);
                    
  PROCEDURE DELETE_USER (I_USER_LOGIN IN USERS.USER_LOGIN%TYPE);
  
  PROCEDURE UPDATE_USER (I_USER_ID IN USERS.USER_ID%TYPE,
                    I_USER_LOGIN IN USERS.USER_LOGIN%TYPE,
                    I_USER_PASSWORD IN USERS.USER_PASSWORD%TYPE);
  
-----------------------------------PLAYLISTS------------------------------------                  
  PROCEDURE CREATE_PLAYLIST (I_PLAYLIST_NAME IN PLAYLISTS.PLAYLIST_NAME%TYPE,
                I_PLAYLIST_DESCRIPTION IN PLAYLISTS.PLAYLIST_DESCRIPTION%TYPE,
                I_USER_ID IN USERS.USER_ID%TYPE,
                I_PLAYLIST_COVER IN PLAYLISTS.PLAYLIST_COVER%TYPE);
                
  PROCEDURE GET_PLAYLIST_BY_USER (I_USER_ID IN USERS.USER_ID%TYPE,
                                O_PLAYLIST_CURS OUT SYS_REFCURSOR);
                                
  PROCEDURE UPDATE_PLAYLIST (I_PLAYLIST_ID IN PLAYLISTS.PLAYLIST_ID%TYPE,
                          I_PLAYLIST_NAME IN PLAYLISTS.PLAYLIST_NAME%TYPE,
                          I_PLAYLIST_DESCRIPTION IN PLAYLISTS.PLAYLIST_DESCRIPTION%TYPE,
                          I_USER_ID IN USERS.USER_ID%TYPE,
                          I_PLAYLIST_COVER IN PLAYLISTS.PLAYLIST_COVER%TYPE);
  
  PROCEDURE DELETE_PLAYLIST (I_PLAYLIST_ID IN PLAYLISTS.PLAYLIST_ID%TYPE);
  
  PROCEDURE ADD_SONG_TO_PLAYLIST (I_SONG_ID IN SONGS.SONG_ID%TYPE,
                      I_PLAYLIST_ID IN PLAYLISTS.PLAYLIST_ID%TYPE);
  
  PROCEDURE DELETE_SONG_FROM_PLAYLIST (I_SONG_ID IN SONGS.SONG_ID%TYPE,
                            I_PLAYLIST_ID IN PLAYLISTS.PLAYLIST_ID%TYPE);
  
---------------------------------FAVORITE_SONG----------------------------------
  PROCEDURE ADD_SONG_TO_FAVOURITE (I_SONG_ID IN SONGS.SONG_ID%TYPE,
                                    I_USER_ID IN USERS.USER_ID%TYPE);
  
  PROCEDURE GET_USER_FAVOURITES (I_USER_ID IN USERS.USER_ID%TYPE,
                                  O_SONGS_CURS OUT SYS_REFCURSOR);
  
  PROCEDURE DELETE_SONG_FROM_FAVOURITE (I_SONG_ID IN SONGS.SONG_ID%TYPE,
                                        I_USER_ID IN USERS.USER_ID%TYPE);
                                        
-------------------------------FOLLOWED_ARTISTS---------------------------------  
  PROCEDURE FOLLOW_ARTIST (I_ARTIST_ID IN ARTISTS.ARTIST_ID%TYPE,
                                  I_USER_ID IN USERS.USER_ID%TYPE);
  
  PROCEDURE UNFOLLOW_ARTIST (I_ARTIST_ID IN ARTISTS.ARTIST_ID%TYPE,
                                  I_USER_ID IN USERS.USER_ID%TYPE);
  
  PROCEDURE GET_FOLLOWED_ARTISTS (I_USER_ID IN USERS.USER_ID%TYPE,
                                  O_ARTISTS_CURS OUT SYS_REFCURSOR);
END;


CREATE OR REPLACE PACKAGE BODY CLIENT_PACKAGE IS
-------------------------------------USERS--------------------------------------

-------------------------REGISTER_USER-------------------------
  PROCEDURE REGISTER_USER (I_USER_LOGIN IN USERS.USER_LOGIN%TYPE, 
                      I_USER_PASSWORD IN USERS.USER_PASSWORD%TYPE)
  IS
  CNT NUMBER;
  BEGIN
    SELECT COUNT(*) INTO CNT FROM USERS WHERE USER_LOGIN = I_USER_LOGIN;
    IF (CNT = 0) THEN
      INSERT INTO USERS (USER_LOGIN, USER_PASSWORD) 
                  VALUES (I_USER_LOGIN, I_USER_PASSWORD);
      COMMIT;
    ELSE
      RAISE_APPLICATION_ERROR(-20001, 'This login is already taken');
    END IF;
END REGISTER_USER;
                      
-------------------------LOG_IN_USER-------------------------
PROCEDURE LOG_IN_USER
  (I_USER_LOGIN IN USERS.USER_LOGIN%TYPE, 
  I_USER_PASSWORD IN USERS.USER_PASSWORD%TYPE,
  O_USER_ID OUT USERS.USER_ID%TYPE,
  O_USER_LOGIN OUT USERS.USER_LOGIN%TYPE) 
IS
  CURSOR USER_CURSOR IS SELECT USER_ID, USER_LOGIN FROM USERS 
    WHERE USERS.USER_LOGIN = I_USER_LOGIN
    AND USERS.USER_PASSWORD = DBMS_CRYPTO.HASH(UTL_RAW.CAST_TO_RAW(I_USER_PASSWORD), 4);
BEGIN
  OPEN USER_CURSOR;
  FETCH USER_CURSOR INTO O_USER_ID, O_USER_LOGIN;
  IF USER_CURSOR%NOTFOUND THEN
    RAISE_APPLICATION_ERROR(-20002, 'Incorrect login or password');
  END IF;
  CLOSE user_cursor;
END LOG_IN_USER;

-------------------------DELETE_USER-------------------------
PROCEDURE DELETE_USER
    (I_USER_LOGIN IN USERS.USER_LOGIN%TYPE)
IS
    CNT NUMBER;
BEGIN
    SELECT COUNT(*) INTO CNT FROM USERS WHERE USERS.USER_LOGIN = I_USER_LOGIN;
    IF (CNT != 0) THEN
        DELETE FROM USERS WHERE USERS.USER_LOGIN = I_USER_LOGIN;
        COMMIT;
    ELSE
        RAISE_APPLICATION_ERROR(-20003, 'User is not found');
    END IF;
END DELETE_USER;
  
-------------------------UPDATE_USER-------------------------
PROCEDURE UPDATE_USER
  (I_USER_ID IN USERS.USER_ID%TYPE,
  I_USER_LOGIN IN USERS.USER_LOGIN%TYPE,
  I_USER_PASSWORD IN USERS.USER_PASSWORD%TYPE)
IS
  CNT NUMBER;
BEGIN
  SELECT COUNT(*) INTO CNT FROM USERS WHERE USERS.USER_ID = I_USER_ID;
  IF (CNT != 0) THEN
    UPDATE USERS SET USER_LOGIN = I_USER_LOGIN,
    USER_PASSWORD = I_USER_PASSWORD 
    WHERE I_USER_ID = USER_ID;
    COMMIT;
  ELSE
    RAISE_APPLICATION_ERROR(-20003, 'User is not found');
  END IF;
END UPDATE_USER;
  
-----------------------------------PLAYLISTS------------------------------------                  
-------------------------CREATE_PLAYLIST-------------------------
PROCEDURE CREATE_PLAYLIST
  (I_PLAYLIST_NAME IN PLAYLISTS.PLAYLIST_NAME%TYPE,
  I_PLAYLIST_DESCRIPTION IN PLAYLISTS.PLAYLIST_DESCRIPTION%TYPE,
  I_USER_ID IN USERS.USER_ID%TYPE,
  I_PLAYLIST_COVER IN PLAYLISTS.PLAYLIST_COVER%TYPE)
IS
  CNT NUMBER;
BEGIN
  SELECT COUNT(*) INTO CNT FROM PLAYLISTS
    WHERE PLAYLIST_NAME = I_PLAYLIST_NAME AND USER_ID = I_USER_ID;
  IF (CNT = 0) THEN
    INSERT INTO PLAYLISTS (PLAYLIST_NAME, PLAYLIST_DESCRIPTION, USER_ID, PLAYLIST_COVER)
      VALUES (I_PLAYLIST_NAME, I_PLAYLIST_DESCRIPTION, I_USER_ID, I_PLAYLIST_COVER);
      COMMIT;
  ELSE
    RAISE_APPLICATION_ERROR(-20010, 'This user already has this playlist');
  END IF;
END CREATE_PLAYLIST;
                
-------------------------GET_PLAYLIST_BY_USER-------------------------
PROCEDURE GET_PLAYLIST_BY_USER
  (I_USER_ID IN USERS.USER_ID%TYPE,
  O_PLAYLIST_CURS OUT SYS_REFCURSOR)
IS
BEGIN
  OPEN O_PLAYLIST_CURS FOR SELECT * FROM PLAYLISTS WHERE USER_ID = I_USER_ID;
  IF O_PLAYLIST_CURS%NOTFOUND THEN
    RAISE_APPLICATION_ERROR(-20020, 'Playlist not found');
  END IF;
END GET_PLAYLIST_BY_USER; 
                                
-------------------------UPDATE_PLAYLIST-------------------------
PROCEDURE UPDATE_PLAYLIST
  (I_PLAYLIST_ID IN PLAYLISTS.PLAYLIST_ID%TYPE,
  I_PLAYLIST_NAME IN PLAYLISTS.PLAYLIST_NAME%TYPE,
  I_PLAYLIST_DESCRIPTION IN PLAYLISTS.PLAYLIST_DESCRIPTION%TYPE,
  I_USER_ID IN USERS.USER_ID%TYPE,
  I_PLAYLIST_COVER IN PLAYLISTS.PLAYLIST_COVER%TYPE)
IS
  CNT NUMBER;
BEGIN
  SELECT COUNT(*) INTO CNT FROM PLAYLISTS
    WHERE PLAYLIST_ID = I_PLAYLIST_ID;
  IF (CNT != 0) THEN
    UPDATE PLAYLISTS SET PLAYLIST_NAME = I_PLAYLIST_NAME,
    PLAYLIST_DESCRIPTION = I_PLAYLIST_DESCRIPTION,
    USER_ID = I_USER_ID, PLAYLIST_COVER = I_PLAYLIST_COVER
    WHERE PLAYLIST_ID = I_PLAYLIST_ID;
      COMMIT;
  ELSE
    RAISE_APPLICATION_ERROR(-20020, 'Playlist not found');
  END IF;
END UPDATE_PLAYLIST;
  
-------------------------DELETE_PLAYLIST-------------------------
PROCEDURE DELETE_PLAYLIST
  (I_PLAYLIST_ID IN PLAYLISTS.PLAYLIST_ID%TYPE)
IS
  CNT NUMBER;
BEGIN
  SELECT COUNT(*) INTO CNT FROM PLAYLISTS
    WHERE PLAYLIST_ID = I_PLAYLIST_ID;
  IF (CNT != 0) THEN
    DELETE FROM PLAYLISTS WHERE PLAYLIST_ID = I_PLAYLIST_ID;
    COMMIT;
  ELSE
    RAISE_APPLICATION_ERROR(-20020, 'Playlist not found');
  END IF;
END DELETE_PLAYLIST;
  
-------------------------ADD_SONG_TO_PLAYLIST-------------------------
PROCEDURE ADD_SONG_TO_PLAYLIST
  (I_SONG_ID IN SONGS.SONG_ID%TYPE,
  I_PLAYLIST_ID IN PLAYLISTS.PLAYLIST_ID%TYPE)
  IS
  CNT NUMBER;
  BEGIN
    SELECT COUNT(*) INTO CNT FROM PLAYLIST_SONG 
      WHERE PLAYLIST_ID = I_PLAYLIST_ID AND SONG_ID = I_SONG_ID;
     IF (CNT = 0) THEN
      INSERT INTO PLAYLIST_SONG(PLAYLIST_ID, SONG_ID) VALUES (I_PLAYLIST_ID, I_SONG_ID);
      COMMIT;
    ELSE
      RAISE_APPLICATION_ERROR(-20011, 'This playlist already has this song');
    END IF;
END ADD_SONG_TO_PLAYLIST;
  
-------------------------DELETE_SONG_FROM_PLAYLIST-------------------------
PROCEDURE DELETE_SONG_FROM_PLAYLIST
  (I_SONG_ID IN SONGS.SONG_ID%TYPE,
  I_PLAYLIST_ID IN PLAYLISTS.PLAYLIST_ID%TYPE)
  IS
  CNT NUMBER;
  BEGIN
    SELECT COUNT(*) INTO CNT FROM PLAYLIST_SONG 
      WHERE PLAYLIST_ID = I_PLAYLIST_ID AND SONG_ID = I_SONG_ID;
     IF (CNT > 0) THEN
      DELETE FROM PLAYLIST_SONG WHERE PLAYLIST_ID = I_PLAYLIST_ID AND SONG_ID = I_SONG_ID;
      COMMIT;
    ELSE
      RAISE_APPLICATION_ERROR(-20012, 'This playlist does not have has this song');
    END IF;
END DELETE_SONG_FROM_PLAYLIST;
  
---------------------------------FAVORITE_SONG----------------------------------
-------------------------ADD_SONG_TO_FAVOURITE-------------------------
PROCEDURE ADD_SONG_TO_FAVOURITE
  (I_SONG_ID IN SONGS.SONG_ID%TYPE,
  I_USER_ID IN USERS.USER_ID%TYPE)
  IS
  CNT NUMBER;
  BEGIN
    SELECT COUNT(*) INTO CNT FROM FAVOURITE_SONGS
      WHERE USER_ID = I_USER_ID AND SONG_ID = I_SONG_ID;
     IF (CNT = 0) THEN
      INSERT INTO FAVOURITE_SONGS(USER_ID, SONG_ID) VALUES (I_USER_ID, I_SONG_ID);
      COMMIT;
    ELSE
      RAISE_APPLICATION_ERROR(-20013, 'This user already has this song in favourite');
    END IF;
END ADD_SONG_TO_FAVOURITE;
  
-------------------------GET_USER_FAVOURITES-------------------------
PROCEDURE GET_USER_FAVOURITES
(I_USER_ID IN USERS.USER_ID%TYPE,
O_SONGS_CURS OUT SYS_REFCURSOR)
IS
BEGIN
  OPEN O_SONGS_CURS FOR
    SELECT SONG_ID, SONG_NAME, SONG_FILE, ARTIST_NAME 
    FROM FAVS_USER_SONG_ARTIST_VIEW
      WHERE USER_ID = I_USER_ID;
END GET_USER_FAVOURITES;
  
-------------------------DELETE_SONG_FROM_FAVOURITE-------------------------
PROCEDURE DELETE_SONG_FROM_FAVOURITE
  (I_SONG_ID IN SONGS.SONG_ID%TYPE,
  I_USER_ID IN USERS.USER_ID%TYPE)
  IS
  CNT NUMBER;
  BEGIN
    SELECT COUNT(*) INTO CNT FROM FAVOURITE_SONGS
      WHERE USER_ID = I_USER_ID AND SONG_ID = I_SONG_ID;
     IF (CNT > 0) THEN
      DELETE FROM FAVOURITE_SONGS WHERE USER_ID = I_USER_ID AND SONG_ID = I_SONG_ID;
      COMMIT;
    ELSE
      RAISE_APPLICATION_ERROR(-20014, 'This user does not have this song in favourite');
    END IF;
END DELETE_SONG_FROM_FAVOURITE;
                                        
-------------------------------FOLLOWED_ARTISTS---------------------------------  
-------------------------FOLLOW_ARTIST-------------------------
PROCEDURE FOLLOW_ARTIST
  (I_ARTIST_ID IN ARTISTS.ARTIST_ID%TYPE,
  I_USER_ID IN USERS.USER_ID%TYPE)
  IS
  CNT NUMBER;
  BEGIN
    SELECT COUNT(*) INTO CNT FROM FOLLOWED_ARTISTS
      WHERE USER_ID = I_USER_ID AND ARTIST_ID = I_ARTIST_ID;
     IF (CNT = 0) THEN
      INSERT INTO FOLLOWED_ARTISTS(USER_ID, ARTIST_ID) VALUES (I_USER_ID, I_ARTIST_ID);
      COMMIT;
    ELSE
      RAISE_APPLICATION_ERROR(-20015, 'This user already has followed this artist');
    END IF;
END FOLLOW_ARTIST;
  
-------------------------UNFOLLOW_ARTIST-------------------------
PROCEDURE UNFOLLOW_ARTIST
  (I_ARTIST_ID IN ARTISTS.ARTIST_ID%TYPE,
  I_USER_ID IN USERS.USER_ID%TYPE)
  IS
  CNT NUMBER;
  BEGIN
    SELECT COUNT(*) INTO CNT FROM FOLLOWED_ARTISTS
      WHERE USER_ID = I_USER_ID AND ARTIST_ID = I_ARTIST_ID;
     IF (CNT > 0) THEN
      DELETE FROM FOLLOWED_ARTISTS WHERE USER_ID = I_USER_ID AND ARTIST_ID = I_ARTIST_ID;
      COMMIT;
    ELSE
      RAISE_APPLICATION_ERROR(-20016, 'This user is not following this artist');
    END IF;
END UNFOLLOW_ARTIST;
  
-------------------------GET_FOLLOWED_ARTISTS-------------------------
PROCEDURE GET_FOLLOWED_ARTISTS
  (I_USER_ID IN USERS.USER_ID%TYPE,
  O_ARTISTS_CURS OUT SYS_REFCURSOR)
  IS
  BEGIN
    OPEN O_ARTISTS_CURS FOR
      SELECT FOLLOWED_ARTISTS.ARTIST_ID FROM FOLLOWED_ARTISTS WHERE FOLLOWED_ARTISTS.USER_ID = I_USER_ID;
  END GET_FOLLOWED_ARTISTS;     
END;