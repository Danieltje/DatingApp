/*
    Interfaces in TS are a little different to interfaces in C#
    When we use interfaces in TS we can specify that something
    is a type of something.

    We here give our interface User two properties username and
    token, and both of type string
*/

export interface User {
    username: string;
    token: string;

    // adding the photoUrl for the main photo we return in the nav
    photoUrl: string;
}
