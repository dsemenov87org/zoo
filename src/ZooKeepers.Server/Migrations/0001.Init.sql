CREATE TABLE IF NOT EXISTS animal
(
    id      SERIAL PRIMARY KEY,
    name    TEXT    NOT NULL,
    imguri  TEXT    NOT NULL
);

CREATE TABLE IF NOT EXISTS user_animal
(
    animalid    INT NOT NULL    REFERENCES animal (id),
    userid      TEXT NOT NULL,
    PRIMARY KEY (animalid, userid)
);