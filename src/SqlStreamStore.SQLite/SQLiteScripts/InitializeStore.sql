

CREATE TABLE IF NOT EXISTS "dbo.Streams"
(
	Id TEXT NOT NULL,
	IdOriginal TEXT NOT NULL,
	IdInternal INTEGER NOT NULL,
	Version INTEGER DEFAULT -1 NOT NULL
);
CREATE UNIQUE INDEX IF NOT EXISTS "dbo.Streams_Id_index" ON "dbo.Streams" (Id);

CREATE TABLE IF NOT EXISTS "dbo.Messages"
(
	StreamIdInternal INTEGER NOT NULL,
	StreamVersion INTEGER NOT NULL,
	Position INTEGER PRIMARY KEY AUTOINCREMENT,
	Id TEXT NOT NULL,
	Created TEXT NOT NULL,
	Type TEXT NOT NULL,
	JsonData TEXT NOT NULL,
	JsonMetadata TEXT,
  CONSTRAINT "FK_Events_Streams" FOREIGN KEY (StreamIdInternal) REFERENCES "dbo.Streams" (IdInternal)
);
-- CREATE UNIQUE INDEX IF NOT EXISTS "IX_Position_uindex" ON "dbo.Messages" (Position);



