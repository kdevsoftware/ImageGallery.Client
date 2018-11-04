import { Injectable } from '@angular/core';
import { Http } from '@angular/http';

import { Response } from '@angular/http';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs/Rx'
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';

import { IUserProfileViewModel } from '../shared/interfaces';

@Injectable()
export class UserManagementService {
    private apiEndpoint = '';

    constructor(
        private http: Http,
        private httpClient: HttpClient
    ) {
        this.getConfig().subscribe(res => {
            this.apiEndpoint = res.json().clientConfiguration.apiUserManagementUri;
        });
    }

    getConfig() {
        return this.http.get('api/ClientAppSettings');
    }

    public getUserInfo() {
        return this.httpClient.get<IUserProfileViewModel>(`/api/UserProfile`)
            .catch(this.handleError);
    }

    resetPassword(email) {
        return this.http.post(`${this.apiEndpoint}/api/Account`, email);
    }

    validatePassword(password) {
        return this.http.post(`${this.apiEndpoint}/api/Account/ValidatePassword`, password);
    }

    createPassword(password) {
        return this.http.post(`${this.apiEndpoint}/api/Account/CreatePassword`, password);
    }

    private handleError(error: any) {
        console.error('server error:', error);
        if (error instanceof Response) {
            let errMessage = '';
            try {
                errMessage = error.json().error;
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