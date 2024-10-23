import { Component } from '@angular/core';
import { ThemeService } from '../services/theme.service';
import { SnackbarService } from '../services/snackbar.service';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { ArticleService } from '../services/article.service';
import { GlobalConstants } from '../shared/global-constants';
import { MatDialogConfig } from '@angular/material/dialog';
import { ArticleDetailsComponent } from '../article-details/article-details.component';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent {
  responseMessage: any;
  articles:any;
  searchText: string = '';

  constructor(public themeService:ThemeService,
    private articleService:ArticleService,
    private snackbarService:SnackbarService,
    private dialog:MatDialog,
    private router: Router,
    private ngxService:NgxUiLoaderService) {
      this.ngxService.start();
      this.tableData();
  }

  tableData(){
    this.articleService.getAllPublishedArticles().subscribe((response:any) => {
      this.articles = response;
      this.ngxService.stop();
    }, (error:any) => {
      this.ngxService.stop()
      console.log(error);
      if(error.error?.message){
        this.responseMessage = error.error?.message;
      }
      else {
        this.responseMessage = GlobalConstants.genericError;
      }
      this.snackbarService.openSnackBar(this.responseMessage);
    }) 
  }

  filterItems(): any {
    return this.articles?.filter(item=>
      item.title.toLowerCase().includes(this.searchText.toLowerCase()) ||
      item.categoryName.toLowercase().includes(this.searchText.toLowerCase()));
  }

  changeTheme(color:any){
    this.themeService.setTheme(color);
  }

  handleViewAction(values:any) {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.data = {
      action: 'Edit',
      data: values
    };
    const dialogRef = this.dialog.open(ArticleDetailsComponent, dialogConfig);
    this.router.events.subscribe(() => {
      dialogRef.close();
    })
  }

}
