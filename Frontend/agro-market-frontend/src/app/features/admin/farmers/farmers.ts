import { Component, OnInit } from '@angular/core';
import { FarmersService } from './farmers.service';
import {CommonModule} from "@angular/common";
import { ChangeDetectorRef } from '@angular/core';

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
      this.cdr.detectChanges();
    });
  }

  select(farmer: any) {
    this.selected = farmer;
  }
}