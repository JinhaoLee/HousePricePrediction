import { Injectable } from '@angular/core';
import { HttpInterceptor } from '@angular/common/http/src/interceptor';
import { HttpRequest } from '@angular/common/http';
import { HttpHandler } from '@angular/common/http/src/backend';
import { Observable } from 'rxjs';
import { HttpEvent } from '@angular/common/http/src/response';
import { environment } from '../../../environments/environment';

const TEST_SERVER = '<API>';

@Injectable()
export class PredictionInterceptor implements HttpInterceptor {
    constructor() {
    }

    /** this will add the authorization to the header */
    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>>{
        if(req.url.includes(TEST_SERVER)){
            req = req.clone({
                url: req.url.replace(TEST_SERVER, environment.predictionServerUrl)
            });
        }
        return next.handle(req);
    }
}
