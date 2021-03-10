import { Directive, Input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { take } from 'rxjs/operators';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';

/* With this directive we're going to be removing or showing an element on the DOM, depending a user is in a certain role.
   We pass parameters to this directive for example: *appHasRole='["Admin"]'. Which user does the user need to be in to access?
*/

@Directive({
  selector: '[appHasRole]' // *ngFor is an example of a "normal" ng directive. This will be a structural directive so *appHasRole
})
export class HasRoleDirective implements OnInit {

  // Taking an Input property to get access to the parameters we give.
  @Input() appHasRole: string[];
  user: User;

  constructor(private viewContainerRef: ViewContainerRef, 
    private templateRef: TemplateRef<any>, 
    private accountService: AccountService) {
      this.accountService.currentUser$.pipe(take(1)).subscribe(user => {
        this.user = user;
      })
     }

  ngOnInit(): void {
    // Clear the view if no roles.
    if(!this.user?.roles || this.user == null) {
      this.viewContainerRef.clear();
      return;
    }

    // Apply a callback function on each element of the array of roles.
    // If the user does have a role that's in that list, we create this embedded view and use that list item as it's reference.
    if(this.user?.roles.some(r => this.appHasRole.includes(r))) {
      this.viewContainerRef.createEmbeddedView(this.templateRef);
    } else {
      this.viewContainerRef.clear();
    }
  }

}
