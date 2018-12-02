import { Component, OnInit, OnDestroy, TemplateRef } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { Subscription } from 'rxjs';
import { RolesConstants } from '../roles.constants';
import { BsModalRef, BsModalService } from 'ngx-bootstrap';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { UserManagementService } from '../services/user.service';
import { IUserProfileViewModel } from '../shared/interfaces';
import { ToastrService } from 'ngx-toastr';
import { StorageService } from '../services/storage.service';

@Component({
  selector: 'app-layout',
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.scss'],
  providers: [AuthService]
})
export class LayoutComponent implements OnInit, OnDestroy {
  isAuthorizedSubscription: Subscription;
  isAuthorized: boolean;
  type: string;
  userType: string;

  isUserInRoleSubscription: Subscription;
  hasPayingUserRole: boolean;
  name = '';

  modalRef: BsModalRef;
  form: FormGroup;

  constructor(
    private authService: AuthService,
    private modalService: BsModalService,
    private userManagementService: UserManagementService,
    private toastr: ToastrService,
    private strage: StorageService) {
    this.form = new FormGroup({
      firstName: new FormControl(['', Validators.required]),
      lastName: new FormControl(['', Validators.required]),
      address: new FormControl(['', Validators.required]),
      address2: new FormControl(['', Validators.required]),
      city: new FormControl(['', Validators.required]),
      country: new FormControl(['', Validators.required]),
      state: new FormControl(['', Validators.required]),
    });
  }

  ngOnInit() {
    console.log(`[ngOnInit]`);
    this.type = 'album';
    this.userType = this.strage.get('currentUser');
    console.log("currentUser", this.userType);

    this.isAuthorizedSubscription = this.authService.getIsAuthorized().subscribe(
      (isAuthorized: boolean) => {
        console.log(`[AuthService] -> [getIsAuthorized] raised with ${isAuthorized}`);

        this.isAuthorized = isAuthorized;
      });

    this.isUserInRoleSubscription = this.authService.checkUserRole(RolesConstants.PayingUser).subscribe(
      (isInRole: boolean) => {
        console.log(`[AuthService] -> [checkUserRole] raised with ${isInRole}`);

        this.hasPayingUserRole = isInRole;
      });

    let userInfo: any = this.authService.getUser();
    this.name = userInfo.given_name + ' ' + userInfo.family_name;

    this.userManagementService.getUserInfo().subscribe((res: IUserProfileViewModel) => {
      if (res) {
        this.name = res.firstName + ' ' + res.lastName;
        this.form.controls.firstName.patchValue(res.firstName);
        this.form.controls.lastName.patchValue(res.lastName);
        this.form.controls.address.patchValue(res.address);
        this.form.controls.address2.patchValue(res.address2);
        this.form.controls.state.patchValue(res.state);
        this.form.controls.city.patchValue(res.city);
        this.form.controls.country.patchValue(res.country);
      }
    });
  }

  ngOnDestroy(): void {
    console.log(`[ngOnDestroy]`)

    this.isAuthorizedSubscription.unsubscribe();
    this.isUserInRoleSubscription.unsubscribe();
  }

  public refreshSession() {
    this.authService.refreshSession();
  }

  public logout() {
    console.log(`[AuthService] -> [logout]`)

    this.authService.logout();
  }

  openModal(template: TemplateRef<any>) {
    this.modalRef = this.modalService.show(template, { class: 'modal-sm' });
  }

  saveUserInfo() {
    const user = this.form.value;
    this.name = this.form.value['firstName'] + ' ' + this.form.value['lastName'];

    this.userManagementService.updateUserInfo(user)
      .subscribe(() => {
        this.toastr.success('User Properties<br/>Successfully Updated', '', { closeButton: true, enableHtml: true });
        this.modalRef.hide();
      });
  }
}
