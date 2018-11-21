import { HousePredictionResponse } from './services/mvc-api/datatypes/API.Entities.HousePredictionResponse';
import { HousePredictionRequest } from './services/mvc-api/datatypes/API.Entities.HousePredictionRequest';
import { PredictionService } from './services/mvc-api/services/API.Controllers.Prediction.Service';
import { Component, NgZone, enableProdMode } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  
  constructor(private zone: NgZone, private prediction: PredictionService){

  }
  initLat: string = "-27.5753528";
  initLng: string = "153.064843";
  initZoom: number = 14;
  NumberOfBedrooms:number = 2;
  NumberOfBathrooms:number = 1;
  NumberOfParkings:number = 0;
  Latitude: number;
  Longitude: number;
  Price: number;

  map: google.maps.Map;

  onMapInit=(map: google.maps.Map)=>{
    this.map = map;
    console.log(this.map);

    this.map.addListener('click', this.onGoogleMapClicked)
  }

  onGoogleMapClicked = (event: google.maps.MouseEvent)=>{
    console.log(event.latLng.lat(), event.latLng.lng());
    this.zone.run(()=>{
      this.Latitude = event.latLng.lat();
      this.Longitude = event.latLng.lng();
    });
  }

  onPredict(){
    var req: HousePredictionRequest = <any>{};
    req.NumberOfBedrooms = this.NumberOfBedrooms;
    req.NumberOfBathrooms = this.NumberOfBathrooms;
    req.NumberOfParkings = this.NumberOfParkings;
    req.Latitude = this.Latitude;
    req.Longitude = this.Longitude;
    this.prediction.Predict(req).subscribe(this.onPredictionResult);
  }

  onPredictionResult = (response: HousePredictionResponse) =>{
    this.Price = response.Price;
  }
}