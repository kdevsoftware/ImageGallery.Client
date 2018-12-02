import { Component, OnInit } from '@angular/core';
import { SettingsService } from '../../../core/settings/settings.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { CustomValidators } from 'ng2-validation';
import { UserManagementService } from '../../../services/user.service';
import { Router } from '@angular/router';
import { TitleService } from '../../../services/title.service';

@Component({
  selector: 'app-recover',
  templateUrl: './recover.component.html',
  styleUrls: ['./recover.component.scss']
})
export class RecoverComponent implements OnInit {

  emailForm: FormGroup;
  passForm: FormGroup;
  resetForm: FormGroup;
  step: number = 1;
  email: string = '';

  constructor(
    public settings: SettingsService,
    fb: FormBuilder,
    public userService: UserManagementService,
    public router: Router,
    private titleService: TitleService) {
    this.emailForm = fb.group({
      'email': [null, Validators.compose([Validators.required, CustomValidators.email])]
    });

    this.passForm = fb.group({
      'password': [null, Validators.compose([Validators.required])]
    });

    this.resetForm = fb.group({
      'password': [null, Validators.compose([Validators.required])],
      'confirmPassword': [null, Validators.compose([Validators.required])],
    });
  }

  submitStep1($ev, value: any) {
    $ev.preventDefault();
    this.email = value.email;
    const len = this.email.indexOf('@') - 1;
    this.email = this.replaceAt(this.email, 1, new Array(len + 1).join('x'));

    for (let c in this.emailForm.controls) {
      this.emailForm.controls[c].markAsTouched();
    }
    if (this.emailForm.valid) {
      this.userService.resetPassword(value).subscribe(res => {
        this.step = 2;
      });
    }
  }

  submitStep2($ev, value: any) {
    $ev.preventDefault();
    for (let c in this.passForm.controls) {
      this.passForm.controls[c].markAsTouched();
    }
    if (this.passForm.valid) {
      this.userService.validatePassword(value).subscribe(res => {
        this.step = 3;
      });
    }
  }

  submitStep3($ev, value: any) {
    $ev.preventDefault();
    for (let c in this.resetForm.controls) {
      this.resetForm.controls[c].markAsTouched();
    }
    if (this.resetForm.valid && value.password === value.confirmPassword) {
      this.userService.createPassword(value).subscribe(res => {
        this.router.navigate(['login']);
      });
    }
  }

  ngOnInit() {
    this.titleService.set('Recover');
  }

  replaceAt(string, index, replace) {
    return string.substring(0, index) + replace + string.substring(index + replace.length);
  }
}
