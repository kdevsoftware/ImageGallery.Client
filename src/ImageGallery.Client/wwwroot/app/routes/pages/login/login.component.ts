import { Component, OnInit } from '@angular/core';
import { SettingsService } from '../../../core/settings/settings.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { OAuthService } from 'angular-oauth2-oidc';
import { Router } from '@angular/router';
import { StorageService } from '../../../services/storage.service';
import { TitleService } from '../../../services/title.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  valForm: FormGroup;
  alertMessage: string;

  constructor(
    public settings: SettingsService,
    private oauthService: OAuthService,
    fb: FormBuilder,
    private router: Router,
    public storage: StorageService,
    private titleService: TitleService) {
    //oath
    if (oauthService.hasValidAccessToken()) {
      this.router.navigate(["home"]);
    }

    //form validation
    this.valForm = fb.group({
      'login': [null, Validators.compose([Validators.required])],
      'password': [null, Validators.required]
    });
  }

  ngOnInit() {
    this.titleService.set('Login');
  }

  submitForm($ev, value: any) {
    $ev.preventDefault();
    for (let c in this.valForm.controls) {
      this.valForm.controls[c].markAsTouched();
    }
    if (this.valForm.valid) {
      this.loginWithPassword(value.login, value.password);
    }
  }

  loginWithPassword(login: string, password: string) {
    console.log(`login: ${login} + password: ${password}`);
    this
      .oauthService
      .fetchTokenUsingPasswordFlowAndLoadUserProfile(login, password)
      .then((res) => {
        console.log(res);

        this.storage.set('currentUser', res['subscriptionlevel']);
        this.router.navigate(['/']);
      })
      .catch((err) => {
        if (err.name === 'TypeError') {
          this.alertMessage = 'Auth Server is Not Reachable';
        } else {
          this.alertMessage = "Invalid request";
        }
      });
  } і
}
