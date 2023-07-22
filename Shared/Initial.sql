
create table Video (
	ID text unique not null, -- GUID
	Title text not null unique,
	MetaTitle text not null,
	MetaSubtitle text not null,
	Length int not null, -- In minutes
	Series text, -- foreign key to Series
	Path text unique not null
);

create table Series (
	ID text unique not null, -- GUID
	Title text not null,
	Description text,
	HideChildren bit
);

create table Thumbnail (
	ID text unique not null,
	[Key] text unique not null,
	RelativePath text
);