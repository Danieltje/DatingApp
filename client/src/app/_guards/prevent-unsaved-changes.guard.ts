import { Injectable } from '@angular/core';
import { CanDeactivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';

@Injectable({
  providedIn: 'root'
})

// implementing the CanDeactivate interface
export class PreventUnsavedChangesGuard implements CanDeactivate<unknown> {
  // we just return a simple Boolean; can they Deactivate? Yes or no
  canDeactivate(component: MemberEditComponent): boolean {
    if (component.editForm.dirty) {

      // if they click Yes here, we say Yes you can deactivate this component; leave the page
      return confirm('Are you sure you want to continue? Any unsaved changes will be lost');
    }
    return true;
  }
}
