import { PredictionService } from './services/mvc-api/services/API.Controllers.Prediction.Service';
import { PredictionInterceptor } from './services/mvc-api/prediction.interceptor';
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { AppComponent } from './app.component';
import { GoogleMapDirective, GoogleMapAPILoader, ErrisyMapModule } from './google-map';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';


@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule, HttpClientModule, FormsModule, ErrisyMapModule.forRoot({
      key: "AIzaSyD0GPb8Trj28E7UnlUGSQNlN1Z8iPAh9iM"
    })
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: PredictionInterceptor,
      multi: true
    },
    PredictionService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
