import { Component, OnInit } from '@angular/core';
import { FarmersService } from './farmers.service';
import {CommonModule} from "@angular/common";
import { ChangeDetectorRef } from '@angular/core';
import co from '@angular/common/locales/co';

@Component({
  selector: 'farmers',
  templateUrl: './farmers.html',
  styleUrls: ['./farmers.css'],
  imports: [CommonModule]
})
export class FarmersComponent implements OnInit {

  farmers: any[] = [];
  selected: any;

  constructor(private service: FarmersService, private cdr: ChangeDetectorRef) {}

  ngOnInit() {
    this.service.getAll().subscribe((res: any) => {
      this.farmers = res;
      console.log(this.farmers);
      this.cdr.detectChanges();
    });
  }

  select(farmer: any) {
    this.selected = farmer;

    this.service
      .getFarmerOrders(farmer.id)
      .subscribe((orders: any) => {

        this.selected.orders = orders;

        this.cdr.detectChanges();
      });
}
}