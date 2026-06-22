-- Create Users table
CREATE TABLE IF NOT EXISTS "Users" (
    "Id" text NOT NULL,
    "Email" character varying(255) NOT NULL,
    "Name" character varying(255),
    "GoogleSubjectId" character varying(255) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT NOW(),
    CONSTRAINT "PK_Users" PRIMARY KEY ("Id")
);

-- Create unique indexes on Users
CREATE UNIQUE INDEX IF NOT EXISTS "IX_Users_Email" ON "Users" ("Email");
CREATE UNIQUE INDEX IF NOT EXISTS "IX_Users_GoogleSubjectId" ON "Users" ("GoogleSubjectId");

-- Create GameProgresses table
CREATE TABLE IF NOT EXISTS "GameProgresses" (
    "UserId" text NOT NULL,
    "Level" integer NOT NULL DEFAULT 0,
    "Score" integer NOT NULL DEFAULT 0,
    "Coins" integer NOT NULL DEFAULT 0,
    "InventoryJson" jsonb NOT NULL DEFAULT '[]'::jsonb,
    "StateDataJson" jsonb NOT NULL DEFAULT '{}'::jsonb,
    "LastUpdated" timestamp with time zone NOT NULL DEFAULT NOW(),
    CONSTRAINT "PK_GameProgresses" PRIMARY KEY ("UserId"),
    CONSTRAINT "FK_GameProgresses_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);
