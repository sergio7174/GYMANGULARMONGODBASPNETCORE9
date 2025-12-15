import { Component, inject,  DestroyRef, OnInit } from '@angular/core';
/**** The takeUntilDestroyed artifact is an operator that unsubscribes
from an observable when the component is destroyed. */
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router } from '@angular/router';
import { PackService } from '../../../../core/services/pack/pack.service';
import { MemberService } from '../../../../core/services/member/member.service';
import { CommonModule } from '@angular/common';
import { ToastrService } from 'ngx-toastr';
import { NgIcon, provideIcons } from '@ng-icons/core';
import { bootstrapPersonCircle, 
         bootstrapHouseAddFill, 
         bootstrapTelephoneForwardFill  } from '@ng-icons/bootstrap-icons';


import { environment } from '../../../../environments/environments';
import { bootstrapChevronRight} from '@ng-icons/bootstrap-icons';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-status',
  standalone: true,
  imports: [
            CommonModule,
            NgIcon, 
            ReactiveFormsModule,
            CardModule,
            InputTextModule,
            ButtonModule,
            NgIcon,
    
],
providers: [provideIcons({ bootstrapPersonCircle, 
                 bootstrapHouseAddFill, 
                 bootstrapTelephoneForwardFill, 
                 bootstrapChevronRight }),
                 ToastrService],
  templateUrl: './status.component.html',
  styleUrl: './status.component.css'
})
export class StatusComponent implements OnInit{

/********** trying to save image new procedure */
selectedFile: File | null = null;
// var to handle preview image
imagePreviewUrl: string = "";

// getting data from localstore

UserName = sessionStorage.getItem('name');
email = sessionStorage.getItem('email');
id = sessionStorage.getItem('id');

// vars to handle daysleft
currentDate: Date = new Date();
today:any = (this.currentDate).getTime();
daysLeft:any=0;
minisecondsLeft:any=0;
Finish_day:any=0;
daysLeft_mathFloor:any=0;
memberStatus: boolean = true;

// vars to handle data from backend
MBEEmail: any = '';
 

// Data that I want to get from URL sended by login component
ItemClassId: string | null = null;

// vars to handle the data from backend
DataMember:any = '';
MemberByEmail: any = [];
mensajeBackend:any="";

// var to handle backend url
baseUrl = environment.endpointI;

private readonly packService = inject(PackService);
private readonly memberService = inject(MemberService);
private readonly router = inject(Router);
private readonly routerParam = inject(ActivatedRoute);
private readonly toast = inject (ToastrService);
//private datePipe= inject(DatePipe);
// Declare the following property to inject the DestroyRef service:
private readonly destroyRef = inject(DestroyRef);



ngOnInit(): void {   this.getPackData(this.email); }

// function to get class data from backend
getPackData(email:any): void {

  //alert("Estoy en status component - line 100 - getClassData - email: "+ email);

  // get the data, to fill the form 
  this.memberService.getmemberByEmail(this.email).pipe(
    takeUntilDestroyed(this.destroyRef)).subscribe((data:any) => {

      this.DataMember = data;
      this.MemberByEmail = this.DataMember.member;
  


     //alert("Estoy en status component - line 111 - this.MemberByEmail.email: " + this.MemberByEmail.email);
     //alert("Estoy en status component component - line 112 - this.MemberByEmail.email: " + this.MemberByEmail.namemember);  
     //alert("Estoy en status component component - line 113 - data.clase.image: " + this.MemberByEmail.imageUser);

           //objectb from backend answer to request
          this.MBEEmail = this.MemberByEmail?.email;
        
  // calc daysleft section 

  this.Finish_day= new Date(this.MemberByEmail.finishAt).getTime();
  this.minisecondsLeft= [(this.Finish_day - this.today)];
        const millisecondsPerDay = 1000 * 60 * 60 * 24;
  this.daysLeft = this.minisecondsLeft/millisecondsPerDay
  this.daysLeft_mathFloor = Math.floor(this.daysLeft);



  // finish calc daysleft section 
 
  }); } // End of GetProduct Data

// function to go back to home
back() {this.router.navigate(['home'],)}


}




