# To build
Open the ./Baozipayment.sln with visual studio 2015 and above (try out the free community version if you don't already have vs)

Build the solution in visual studio

# To Publish
Right click the Baozipayment *project* in visual studio, and click the *publish* button.
In the popup window, click the *Connection* tab.
It should prefilled with all the accounts info. You need to type in our baozi password.
Click publish to publish

# To update the email message
In the NotificationProcessor.cs file under the Models folder, you should be able to see multiple examples regarding to sending emails for different events.
In particular, the *stateTransite* function should be paid attention to.

The *emailNotifyUser* funciton will send email with certain templates, and all templates are defined in the resource file.

Right click the *Baozipayment* project and click properties. On the settings tab, you should be able to see all the existing template resources.
You can also modify those strings and save it.

Update the template you needed, and adjust the code accordingly.
# To sync from gbb21/baozipayment
I forked a repo, sometimes the original contains changes I want so I need to sync back. 
```
git remote add upsteam https://github.com/gbb21/baozipayment.git
git fetch upsteam
git checkout master
git rebase upstream/mastere
```
