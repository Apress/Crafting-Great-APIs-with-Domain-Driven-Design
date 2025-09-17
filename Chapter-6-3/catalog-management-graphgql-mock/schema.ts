import { createSchema } from 'graphql-yoga'
import { v4 as uuidv4 } from 'uuid'
 
const books = [
  {
    identifier: '0ed4a696-793f-4a89-8069-15df09b56e4f',
    isbn13: '978-3-16-148410-0',
    title: 'The Great Gatsby',
    authors: ['F. Scott Fitzgerald'],
    numberOfTextPositions: 180,
    category: 'FICTION',
    tags: ['classic', 'American literature'],
    rating: 4.5,
    publishingDate: '1925-04-10',
    abstract: 'A novel set in the 1920s that tells the story of Jay Gatsby, a mysterious millionaire, and his obsession with the beautiful Daisy Buchanan.'
  },
  {
    identifier: '9214c1e2-dff7-4c74-975c-cc207d8109c7',
    isbn13: '978-0-7432-7356-5',
    title: 'The Da Vinci Code',
    authors: ['Dan Brown'],
    numberOfTextPositions: 450,
    category: 'FICTION',
    tags: ['mystery', 'thriller'],
    rating: 4.2,
    publishingDate: '2003-03-18',
    abstract: 'A mystery thriller that follows symbologist Robert Langdon as he uncovers a conspiracy involving the Catholic Church and a secret society.'
  }
]

export const schema = createSchema({
  typeDefs: /* GraphQL */ `
    "A book as entry in the catalog."
    type Book {
        "Identifier of the catalog entry"
        identifier: ID!
        "The 13-digit ISBN"
        isbn13: String!
        "Title"
        title: String
        "List of authors of the book - the list cannot be empty"
        authors: [String!]!
        "Number of text positions in the book"
        numberOfTextPositions: Int
        "Category"
        category: Catagory
        "Top twenty of individual tags give to the particular book"
        tags: [String]
        "Rating of the book between 1 and 5"
        rating: Float
        "Date of publishing"
        publishingDate: String!
        "Abstract written by a librarian"
        abstract: String
    }

    "A book as entry in the catalog to be created."
    input BookInput {
        "The 13-digit ISBN"
        isbn13: String!
        "Title"
        title: String
        "List of authors of the book - the list cannot be empty"
        authors: [String!]!
        "Number of text positions in the book"
        numberOfTextPositions: Int
        "Category"
        category: Catagory
        "Top twenty of individual tags give to the particular book"
        tags: [String]
        "Rating of the book between 1 and 5"
        rating: Float
        "Date of publishing"
        publishingDate: String!
        "Abstract written by a librarian"
        abstract: String
    }

    enum Catagory {
        NON_FICTION,
        FICTION
    }

    type Query {
				"Delivers all books fulfilling the searchCriteria"
				catalogEntries(searchCriteria: String): [Book!]
				"Delivers a book based on its ID"
				catalogEntry(identifier: ID!): Book
			}

    type Mutation {
      "Creates a new entry of a book in the catalog"
      createNewEntry(book: BookInput!): Book!
      "Updates a catalog entry"
      updateCatalogEntry(identifier: ID!, book: BookInput!): Book!
      "Creates and updates an abstract of a book - deleting means an empty abstract"
      createUpdateAbstract(identifier: ID!, abstract: String): ID!
    }
  `,
  resolvers: { 
    Query: {
      catalogEntries: () => books,
      catalogEntry: (_, { identifier }) => books.find(book => book.identifier === identifier)
    },
    Mutation: {
      createNewEntry: (_, { book }) => {
        const newBook = { ...book, identifier: uuidv4() };
        books.push(newBook);
        return newBook;
      },
      updateCatalogEntry: (_, { identifier, book }) => {
        const index = books.findIndex(b => b.identifier === identifier);
        if (index === -1) throw new Error('Book not found');
        books[index] = { ...books[index], ...book };
        return books[index];
      },
      createUpdateAbstract: (_, { id, abstract }) => {
        const book = books.find(b => b.identifier === id);
        if (!book) throw new Error('Book not found');
        book.abstract = abstract || '';
        return id;
      }
    }
  }
});
