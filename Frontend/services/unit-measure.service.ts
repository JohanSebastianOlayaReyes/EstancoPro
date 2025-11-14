import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { UnitMeasureDto } from '../models/unit-measure.model';

@Injectable({
  providedIn: 'root'
})
export class UnitMeasureService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/UnitMeasure`;

  getAll(): Observable<UnitMeasureDto[]> {
    return this.http.get<UnitMeasureDto[]>(this.apiUrl);
  }

  getById(id: number): Observable<UnitMeasureDto> {
    return this.http.get<UnitMeasureDto>(`${this.apiUrl}/${id}`);
  }

  create(unitMeasure: UnitMeasureDto): Observable<UnitMeasureDto> {
    return this.http.post<UnitMeasureDto>(this.apiUrl, unitMeasure);
  }

  update(unitMeasure: UnitMeasureDto): Observable<UnitMeasureDto> {
    return this.http.put<UnitMeasureDto>(`${this.apiUrl}/${unitMeasure.id}`, unitMeasure);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
