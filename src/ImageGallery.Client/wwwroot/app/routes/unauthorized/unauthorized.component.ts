import { Component, OnInit } from '@angular/core';
import { Location } from '@angular/common';
import { AuthService } from '../../services/auth.service';
import { TitleService } from '../../services/title.service';

@Component({
  selector: 'app-unauthorized',
  templateUrl: 'unauthorized.component.html'
})
export class UnauthorizedComponent implements OnInit {

  constructor(
    private location: Location,
    private authService: AuthService,
    private titleService: TitleService) { }

  ngOnInit() {
    this.titleService.set('Unauthorized');
  }

  login() {
    this.authService.login();
  }

  goback() {
    this.location.back();
  }
}