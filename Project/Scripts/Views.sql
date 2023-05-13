ALTER SESSION SET "_ORACLE_SCRIPT" = TRUE;
select * from user_views;

-------------------------ARTIST & ALBUM & SONG-------------------------
CREATE OR REPLACE VIEW ARTIST_ALBUM_SONG_VIEW AS 
  SELECT ARTISTS.ARTIST_ID, ARTISTS.ARTIST_NAME, 
  ALBUMS.ALBUM_ID, ALBUMS.ALBUM_NAME, ALBUMS.ALBUM_RELEASE_DATE, ALBUMS.ALBUM_COVER,
  SONGS.SONG_ID, SONGS.SONG_NAME, SONGS.SONG_FILE
  FROM ALBUMS JOIN ALBUM_SONG ON ALBUMS.ALBUM_ID = ALBUM_SONG.ALBUM_ID  
  JOIN SONGS ON ALBUM_SONG.SONG_ID = SONGS.SONG_ID
  JOIN ARTISTS ON SONGS.ARTIST_ID = ARTISTS.ARTIST_ID;

-------------------------PLAYLIST & USER-------------------------
CREATE OR REPLACE VIEW PLAYLIST_USER_VIEW AS
  SELECT USERS.USER_ID, USERS.USER_LOGIN, 
  PLAYLISTS.PLAYLIST_ID, PLAYLISTS.PLAYLIST_NAME, 
  PLAYLISTS.PLAYLIST_DESCRIPTION, PLAYLISTS.PLAYLIST_COVER
  FROM PLAYLISTS JOIN USERS ON USERS.USER_ID = PLAYLISTS.USER_ID;

-------------------------PLAYLIST & SONG-------------------------
CREATE OR REPLACE VIEW PLAYLIST_SONG_VIEW AS
  SELECT SONGS.SONG_ID, SONGS.SONG_NAME, SONGS.SONG_FILE, SONGS.ARTIST_ID,
  PLAYLISTS.PLAYLIST_ID, PLAYLISTS.PLAYLIST_NAME, 
  PLAYLISTS.PLAYLIST_DESCRIPTION, PLAYLISTS.PLAYLIST_COVER
  FROM SONGS JOIN PLAYLIST_SONG ON SONGS.SONG_ID = PLAYLIST_SONG.SONG_ID
  JOIN PLAYLISTS ON PLAYLISTS.PLAYLIST_ID = PLAYLIST_SONG.PLAYLIST_ID;

-------------------------FAVOURITE_SONG & USER & SONG & ARTIST-----------------------
CREATE OR REPLACE VIEW FAVS_USER_SONG_ARTIST_VIEW AS
  SELECT USERS.USER_ID, USERS.USER_LOGIN,
    SONGS.SONG_ID, SONGS.SONG_NAME, SONGS.SONG_FILE,
    ARTISTS.ARTIST_ID, ARTISTS.ARTIST_NAME
    FROM USERS JOIN FAVOURITE_SONGS ON USERS.USER_ID = FAVOURITE_SONGS.USER_ID
    JOIN SONGS ON SONGS.SONG_ID = FAVOURITE_SONGS.SONG_ID
    JOIN ARTISTS ON ARTISTS.ARTIST_ID = SONGS.ARTIST_ID;

-------------------------FAVORITE_SONG_ID_COUNT_VIEW-----------------------
CREATE OR REPLACE VIEW FAVORITE_SONG_ID_COUNT_VIEW AS
  SELECT SONG_ID, COUNT(USER_ID) AS FAVORITE_COUNT
      FROM FAVS_USER_SONG_ARTIST_VIEW
      GROUP BY SONG_ID
      ORDER BY FAVORITE_COUNT DESC;

-------------------------TOP_FAVOURITE_SONGS-----------------------
CREATE OR REPLACE VIEW TOP_FAVOURITE_SONGS_VIEW AS
  SELECT FAVORITE_SONG_ID_COUNT_VIEW.SONG_ID, 
    SONG_NAME, SONG_FILE, ARTIST_NAME 
    FROM  FAVORITE_SONG_ID_COUNT_VIEW JOIN FAVS_USER_SONG_ARTIST_VIEW
    ON FAVORITE_SONG_ID_COUNT_VIEW.SONG_ID = FAVS_USER_SONG_ARTIST_VIEW.SONG_ID;
    
-------------------------TOP_FOLLOWED_ARTISTS-----------------------
CREATE OR REPLACE VIEW TOP_FOLLOWED_ARTISTS_VIEW AS
   SELECT ARTIST_ID, COUNT(USER_ID) AS FOLLOWERS_COUNT 
   FROM FOLLOWED_ARTISTS
  GROUP BY ARTIST_ID
  ORDER BY FOLLOWERS_COUNT DESC;
  
-------------------------USER & SONG & FOLLOWED_ARTIST-----------------------
CREATE OR REPLACE VIEW USER_SONG_FOLLOWED_ARTIST_VIEW AS
  SELECT USERS.USER_ID, USERS.USER_LOGIN,
    SONGS.SONG_ID, SONGS.SONG_NAME, SONGS.SONG_FILE,
    ARTISTS.ARTIST_ID, ARTISTS.ARTIST_NAME
    FROM USERS JOIN FOLLOWED_ARTISTS ON USERS.USER_ID = FOLLOWED_ARTISTS.USER_ID
    JOIN ARTISTS ON ARTISTS.ARTIST_ID = FOLLOWED_ARTISTS.ARTIST_ID
    JOIN SONGS ON SONGS.ARTIST_ID = ARTISTS.ARTIST_ID;