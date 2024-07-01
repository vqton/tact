CREATE TABLE landing_page (
    page_id INTEGER PRIMARY KEY,
    page_title TEXT NOT NULL,
    url TEXT NOT NULL,
    description TEXT,
    cta TEXT,
    conversion_rate REAL,
    notes TEXT
);

CREATE TABLE sections (
    section_id INTEGER PRIMARY KEY,
    page_id INTEGER NOT NULL,
    section_name TEXT NOT NULL,
    FOREIGN KEY (page_id) REFERENCES landing_page (page_id)
);

CREATE TABLE custom_fields (
    field_id INTEGER PRIMARY KEY,
    section_id INTEGER NOT NULL,
    field_name TEXT NOT NULL,
    field_value TEXT,
    FOREIGN KEY (section_id) REFERENCES sections (section_id)
);
