import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve, RouterStateSnapshot } from "@angular/router";
import { Observable } from "rxjs";
import { Member } from "../_models/member";
import { MembersService } from "../_services/members.service";

// This is not a component, so we need to bring the Injectable decorator in this
// Resolvers are instantiated in the same way as services really
@Injectable({
    providedIn: 'root'
})

export class MemberDetailedResolver implements Resolve<Member> {

    // Inject the memberService, because we need our Member
    constructor(private memberService: MembersService) {}

    // We don't need to subscribe in route resolvers. The Router is gonna take care of this for us.
    // All we do is specify the method in our service that returns an Observable and the Router will sub and unsub.
    // This is a method to get your data before you construct your template, for fixes like these.
    resolve(route: ActivatedRouteSnapshot): Observable<Member> {
        return this.memberService.getMember(route.paramMap.get('username'));
    }
    
}