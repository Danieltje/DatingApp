import { User } from "./user";

export class UserParams {
    // Giving it initial parameters we can use when we instantiate a new instance of this class
    gender: string;
    minAge = 18;
    maxAge = 99;
    pageNumber = 1;
    pageSize = 5;
    orderBy = 'lastActive';

    constructor(user: User) {
        this.gender = user.gender === 'female' ? 'male' : 'female';
    }
}