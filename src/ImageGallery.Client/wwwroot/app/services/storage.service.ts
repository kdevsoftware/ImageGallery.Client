import { Injectable } from '@angular/core';
import { AuthService } from './auth.service';

@Injectable()
export class StorageService {
  constructor(private authService: AuthService) { }

  set(key: string, value: string): void {
    localStorage.setItem(key, value);
  }

  setPerUser(key: string, value: string): void {
    const userKey = this.getUserKey(key);
    localStorage.setItem(userKey, value);
  }

  get(key: string): string {
    return localStorage.getItem(key);
  }

  getPerUser(key: string): string {
    
    const userKey = this.getUserKey(key);
    return localStorage.getItem(userKey);
  }

  remove(key: string) {
    localStorage.removeItem(key);
  }

  private getUserKey(key) {
    const user: any = this.authService.getUser();
    const userName = user.sub;
    return userName + '_' + key;
  }
}