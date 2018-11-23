import { Component, AfterViewInit } from '@angular/core';
import { SettingsService } from '../../../core/settings/settings.service';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { CustomValidators } from 'ng2-validation';
import { Router } from '@angular/router';

import { ReCaptchaService, ReCaptchaParamsInterface } from '../../../reCaptchaCallback'

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements AfterViewInit {
  valForm: FormGroup;
  passwordForm: FormGroup;

  constructor(public fb: FormBuilder
    , private router: Router
    , private reCaptchaService: ReCaptchaService
    , public settings: SettingsService) {

    let password = new FormControl('', Validators.compose([Validators.required, Validators.pattern('^[a-zA-Z0-9]{6,10}$')]));
    let certainPassword = new FormControl('', CustomValidators.equalTo(password));

    this.passwordForm = fb.group({
      'password': password,
      'confirmPassword': certainPassword
    });

    this.valForm = fb.group({
      'userId': [null, Validators.required],
      'email': [null, Validators.compose([Validators.required, CustomValidators.email])],
      'lname': [null, Validators.required],
      'fname': [null, Validators.required],
      'accountagreed': [null, Validators.required],
      'passwordGroup': this.passwordForm,
      'ggl-recaptcha-input': [null, Validators.required]
    });
  }

  ngAfterViewInit(): void {
    this.reCaptchaConfig();
  }

  private reCaptchaConfig() {
    var self = this;

    if (this.reCaptchaService.recaptcha_is_disabled) {
      setTimeout(() => { self.valForm.controls['ggl-recaptcha-input'].setValue("recaptcha not enabled") }, 0);
      return;
    }

    var reCaptchaElement: HTMLElement = <HTMLElement>document.getElementById("ggl-recaptcha");
    if (reCaptchaElement) {
      let params: ReCaptchaParamsInterface = {
        sitekey: this.reCaptchaService.recapthcaClientKey,
        callback: (recaptchaToken) => {
          self.valForm.controls['ggl-recaptcha-input'].setValue(recaptchaToken);
        },
        'expired-callback': () => {
          console.warn("recaptcha is expired");
          this.reCaptchaService.reset();
          self.valForm.controls['ggl-recaptcha-input'].setValue(null);
        },
        'error-callback': () => {
          console.error("recaptcha error");
          this.reCaptchaService.reset();
          self.valForm.controls['ggl-recaptcha-input'].setValue(null);
        }
      }
      this.reCaptchaService.render(reCaptchaElement, params);
    }
  }

  submitForm($ev, value: any) {
    $ev.preventDefault();
    for (let c in this.valForm.controls) {
      this.valForm.controls[c].markAsTouched();
    }
    for (let c in this.passwordForm.controls) {
      this.passwordForm.controls[c].markAsTouched();
    }

    if (!this.valForm.valid || value.passwordGroup.password !== value.passwordGroup.confirmPassword) return;

    // let headers = new Headers();
    // headers.append('Content-Type', 'application/json');
    // let options = new RequestOptions({ headers: headers });

    // let body = {
    //   Email: value.email,
    //   Password: value.passwordGroup.password,
    //   RecaptchaToken: value["ggl-recaptcha-input"]
    // };

    console.error("No registration url was provided");
    /*
    this.http.post(OAuthSettings.api_user_management_registration_url, body, options)
      .subscribe(
      res => {
        console.dir(res);
        this.router.navigate(['/login']);
      }
      );
    */
  }
}
