import { Injectable } from '@angular/core';

import { HttpClient, HttpErrorResponse } from '@angular/common/http';

import { Observable } from 'rxjs/Rx'
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';

import { IUserProfileViewModel } from '../shared/interfaces';

@Injectable()
export class UserManagementService {
    private apiEndpoint = '';

    constructor(
        private httpClient: HttpClient
    ) {
        this.getConfig().subscribe((res: any) => {
            this.apiEndpoint = res.clientConfiguration.apiUserManagementUri;
        });
    }

    getConfig() {
        return this.httpClient.get('api/ClientAppSettings');
    }

    public getUserInfo() {
        return this.httpClient.get<IUserProfileViewModel>(`/api/UserProfile`)
            .catch(this.handleError);
    }

    public updateUserInfo(userModel: IUserProfileViewModel) {
       return this.httpClient.put<IUserProfileViewModel>('/api/UserProfile', userModel);
    }

    getCountries() {
        return this.httpClient.get(`https://user-management.informationcart.com/api/Reference/countries`);
    }

    getLanguages() {
        return this.httpClient.get(`https://user-management.informationcart.com/api/Reference/languages`);
    }

    resetPassword(email) {
        return this.httpClient.post(`${this.apiEndpoint}/api/Account`, email);
    }

    validatePassword(password) {
        return this.httpClient.post(`${this.apiEndpoint}/api/Account/ValidatePassword`, password);
    }

    createPassword(password) {
        return this.httpClient.post(`${this.apiEndpoint}/api/Account/CreatePassword`, password);
    }

    private handleError(error: any) {
        console.error('server error:', error);
        if (error instanceof HttpErrorResponse) {
            let errMessage = '';
            try {
                errMessage = error.error;
            } catch (err) {
                errMessage = error.statusText;
            }
            return Observable.throw(errMessage);
            // Use the following instead if using lite-server
            //return Observable.throw(err.text() || 'backend server error');
        }
        return Observable.throw(error || 'ASP.NET Core server error');
    }
}
