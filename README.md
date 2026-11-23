# KeePassPacupColumns

KeePass 2.x plug-in, adding 5 columns to the main entry list, based on the QualityColumn/ColumnProvider plug-in.

- 'Part' column identifies the parent record of a child record. Its type is text or entry-UUID.
- 'Attn' column marks records needing special attention. Its value is `1/true/yes/y/x` shown as `!` or `0/false/no/n` shown as blank.
- 'Caty' column helps in categorizing the records. Its type is text.
- 'User' column has names of users for user-specific records. Its type is text.
- 'Pins' column gives security codes for retrieving the password. It is masked `********` unless Tools > "Show PINs in the 'Pins' Column" is checked.

## Build

- Open KeePassPacupColumns.csproj in Visual Studio; fix the `KeePass` reference HintPath to your KeePass.exe.
- Build Release, then either copy KeePassPacupColumns.dll into KeePass' Plugins folder or zip the source folder and rename to `.plgx`.

## Usage

- Set the values per entry: Edit Entry > Advanced > String fields.
- Show the columns via View > Configure Columns.
- Columns are read-only; sorting works on the displayed text.
