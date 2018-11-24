import { Injectable } from '@angular/core';

import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/catch';

import { HttpClient, HttpErrorResponse } from '@angular/common/http';

@Injectable()
export class AuthenticationService {

    baseUrl: string = '/api/images';

    constructor(private httpClient: HttpClient) {
    }

    public logout(): Observable<Object> {
        return this.httpClient.get(`${this.baseUrl}/logout`)
            .catch(this.handleError);
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