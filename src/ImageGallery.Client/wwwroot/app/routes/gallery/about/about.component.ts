import { Component, OnInit } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';
import { TitleService } from '../../../services/title.service';


@Component({
  selector: 'app-about',
  templateUrl: './about.component.html',
  styleUrls: ['./about.component.scss']
})
export class AboutComponent implements OnInit {

  public id_token: string;
  public access_token: string;
  public userData: any;

  constructor(
    private oauthService: OAuthService,
    private titleService: TitleService) { }

  ngOnInit() {
    this.titleService.set('About');
    
    this.id_token = this.oauthService.getIdToken();
    this.access_token = this.oauthService.getAccessToken();
    this.userData = this.oauthService.getIdentityClaims();

    console.log(`User data: ${this.userData}`);
  }

}
