import { Injectable } from '@angular/core';
import { CanDeactivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';
import { ConfirmService } from '../_services/confirm.service';

@Injectable({
  providedIn: 'root'
})

// implementing the CanDeactivate interface
export class PreventUnsavedChangesGuard implements CanDeactivate<unknown> {

  constructor(private confirmService: ConfirmService) {}

  // we just return a simple Boolean; can they Deactivate? Yes or no
  canDeactivate(component: MemberEditComponent): Observable<boolean> | boolean  {
    if (component.editForm.dirty) {
      return this.confirmService.confirm()
    }
    return true;
  }
}
